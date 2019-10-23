using Mel.Live.Data;
using Mel.Live.Extensions.UnityExtensions;
using Mel.Live.Models;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Options;
using Mel.Live.Models.Responses;
using Mel.Live.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mel.Live.Services
{
    [AutoBuild]
    public class PaymentService : IPaymentService
    {
        #region Properties

        #region Services
        [DeepDependency]
        ILogger Logger { get; }

        [DeepDependency]
        MongoDataContext DataContext { get; }

        [DeepDependency]
        IPayStackApi Paystack { get; }
        #endregion

        #region Options
        [DeepDependency(TargetType = typeof(IOptions<PaystackOptions>), 
            TargetProperty = nameof(IOptions<PaystackOptions>.Value))]
        PaystackOptions PaystackOptions { get; }
        #endregion

        #endregion

        #region Methods

        #region Paystack
        public async Task<PaymentResult> UsePaystack(User user, decimal amount)
        {
            bool customerExists = false;

            try { customerExists = Paystack.Customers.Fetch(user.Email).Status; }
            catch { }

            if (!customerExists)
            {
                Paystack.Customers.Create(new CustomerCreateRequest()
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Phone = user.PhoneNumber
                });
            }

            PaymentResult result = new PaymentResult();
            bool isRecurring = !string.IsNullOrWhiteSpace(user.Wallet.PaystackAuthorization);
            PaymentChannel channel = await DataContext.Store.GetOneAsync<PaymentChannel, int>(p => p.Type == ChannelType.Paystack);

            // Convert to kobo
            int total = (int)Math.Round(amount * channel.ConversionRate);

            if (isRecurring)
            {
                var success = Paystack.Transactions
                    .ChargeAuthorization(user.Wallet.PaystackAuthorization, user.Email, 
                        total, user.Wallet.ToString(), false);

                if (success.Status)
                {
                    result.Status = PaymentStatus.Success;
                    return result;
                }
            }

            var request = new TransactionInitializeRequest()
            {
                Email = user.Email,
                AmountInKobo = total,
                Reference = Guid.NewGuid().ToString(),
                MetadataObject = new Dictionary<string, object>()
                {
                    { "walletId", user._Wallet.ToString() }
                }
            };

            var response = Paystack.Transactions.Initialize(request);

            result.Status = response.Status ? PaymentStatus.Redirected : PaymentStatus.Failed;
            result.Message = response.Data.AuthorizationUrl;

            if (result.Status == PaymentStatus.Failed)
            {
                Logger.LogError("A Paystack Transaction appears to have failed. \n{0}", response.Message);
                result.Message = response.Message;
            }

            return result;
        }

        public Task<bool> ValidatePaystackSignature(string signature, string request)
        {
            byte[] key = Encoding.UTF8.GetBytes(PaystackOptions.SecretKey);
            byte[] input = Encoding.UTF8.GetBytes(request);
            string paystackSignature = string.Empty;

            using (var hmac = new HMACSHA512(key))
            {
                byte[] hash = hmac.ComputeHash(input);
                paystackSignature = BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            bool valid = paystackSignature.ToLower() == signature.ToLower();
            return Task.FromResult(valid);
        }

        public async Task<bool> ConcludePaystack(PaystackTransactionData data, PaymentType type, UserTransactionMetadata meta = null)
        {
            if (type != PaymentType.Deposit && type != PaymentType.Withdrawal && type != PaymentType.Direct)
                return false;

            PaymentChannel channel = await DataContext.Store
                .GetOneAsync<PaymentChannel, int>(p => p.Type == ChannelType.Paystack);

            if (!decimal.TryParse(data.Amount, out decimal transactionAmount)) return false;

            if (!Guid.TryParse((string)data.Metadata["walletId"], out Guid walletId)) return false;
            if (!Guid.TryParse(data.Reference, out Guid transactionId)) return false;

            Wallet wallet = await DataContext.Store.GetByIdAsync<Wallet>(walletId);

            decimal amount = transactionAmount / channel.ConversionRate;
            await CreateTransaction(wallet, amount, type, ChannelType.Paystack, Status.Approved, meta);

            if (data.Authorization.Reusable && string.IsNullOrWhiteSpace(wallet.PaystackAuthorization))
            {
                wallet.PaystackAuthorization = data.Authorization.AuthorizationCode;
                await DataContext.Store.UpdateOneAsync(wallet.User);
            }

            return true;
        }
        #endregion

        #region Bank
        public async Task<PaymentResult> UseBank(BankTransactionRequest request, PaymentType type)
        {
            if (type != PaymentType.Deposit && type != PaymentType.Withdrawal)
                return new PaymentResult() { Status = PaymentStatus.Failed, Message = "Payment type is not supported" };

            Wallet wallet = await DataContext.Store.GetByIdAsync<Wallet>(request.User._Wallet);
            await CreateTransaction(wallet, request.Amount, type, ChannelType.Bank, Status.Pending, request.Meta);

            return new PaymentResult() { Status = PaymentStatus.Pending };
        }
        #endregion

        #region Transactions
        async Task<bool> CreateTransaction(Wallet wallet, decimal amount, 
            PaymentType type, ChannelType channelType, 
            Status status, UserTransactionMetadata meta = null)
        {
            Transaction transaction = new Transaction()
            {
                AddedAtUtc = DateTime.UtcNow,
                Amount = amount,
                Status = status,
                PaymentType = type,
                _PaymentChannel = (int)channelType,
            };

            if (meta != null) transaction.Meta = meta;

            await transaction.InitializeAsync();

            await wallet.ProcessTransaction(transaction);
            return true;
        }


        #endregion

        #endregion
    }
}
