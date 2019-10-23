using Mel.Live.Models;
using Mel.Live.Models.Entities;
using Mel.Live.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResult> UsePaystack(User user, decimal amount);
        Task<bool> ValidatePaystackSignature(string signature, string request);
        Task<bool> ConcludePaystack(PaystackTransactionData data, PaymentType type, UserTransactionMetadata meta = null);

        Task<PaymentResult> UseBank(BankTransactionRequest request, PaymentType type);
    }
}
