using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("Wallets")]
    public class Wallet : DocumentBase
    {
        public decimal Balance { get; set; } = 0M;
        public decimal AvailableBalance { get; set; } = 0M;

        [BsonIgnoreIfDefault]
        public string PaystackAuthorization { get; set; }

        #region References
        public Guid _User { get; set; }
        public List<Guid> _Transactions { get; set; } = new List<Guid>();
        public List<Guid> _TransactionRequests { get; set; } = new List<Guid>();
        #endregion

        #region Links
        User @user;
        [BsonIgnore]
        public User User
        {
            get
            {
                if (@user == null)
                    @user = Core.DataContext.Store.GetById<User>(_User);
                return user;
            }
        }

        IEnumerable<Transaction> @transactions;
        [BsonIgnore]
        public IEnumerable<Transaction> Transactions
        {
            get
            {
                if (@transactions == null)
                    @transactions = Core.DataContext.Get<Transaction>(_Transactions);
                return @transactions;
            }
        }
        #endregion

        #region Constructors
        public Wallet() { }
        public Wallet(User user) { _User = user.Id; }
        #endregion

        #region Methods
        
        public async Task ProcessTransaction(Transaction transaction)
        {
            if (transaction == null) throw new InvalidOperationException("Transaction cannot be null");

            // if (transaction.Meta.UserId != Guid.Empty) throw new InvalidOperationException("Transaction has already been processed");
            // if (_Transactions.Any(id => id == transaction.Id)) throw new InvalidOperationException("Transaction has already been processed");
            if (transaction.Amount < 0) throw new InvalidOperationException("Invalid transaction amount.");

            decimal balance = Balance;
            decimal availableBalance = AvailableBalance;
            decimal amount = transaction.Amount;

            switch (transaction.PaymentType)
            {
                case PaymentType.Deposit:
                case PaymentType.Credit:
                    if (transaction.Status != Status.Approved) break;
                    balance += amount;
                    availableBalance += amount;
                    break;

                case PaymentType.Withdrawal:
                case PaymentType.Debit:
                    balance -= amount;
                    availableBalance -= amount;

                    if (balance < 0) throw new InvalidOperationException("Insufficient funds");
                    break;
            }


            Balance = balance;
            AvailableBalance = availableBalance;

            transaction.Meta.UserId = _User;
            await Core.DataContext.Store.AddOneAsync(transaction);

            _Transactions.Add(transaction.Id);
            await Core.DataContext.Store.UpdateOneAsync(this);
        }

        #endregion
    }
}
