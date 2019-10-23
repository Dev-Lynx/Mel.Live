using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("TransactionRequests")]
    public class TransactionRequest : DocumentBase
    {
        #region Properties
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }

        public string Description { get; set; }
        public Status Status { get; set; }
        public PaymentType TransactionType { get; set; }

        public UserTransactionRequestMetadata Meta { get; set; }

        #region References
        public int _PaymentChannel { get; set; }
        #endregion

        #region Links
        User @user;
        [BsonIgnore]
        public User User
        {
            get
            {
                if (@user == null && Meta != null)
                    @user = Core.DataContext.Store.GetById<User>(Meta.UserId);
                return @user;
            }
        }

        PaymentChannel @paymentChannel;
        [BsonIgnore]
        public PaymentChannel PaymentChannel
        {
            get
            {
                if (@paymentChannel == null)
                    @paymentChannel = Core.DataContext.Store.GetById<PaymentChannel, int>(_PaymentChannel);
                return @paymentChannel;
            }
        }
        #endregion

        #endregion
    }

    /*
    public class BankTransactionRequest : TransactionRequest
    {
        #region Properties

        #region References
        public Guid _UserBankAccount { get; set; }
        public Guid _PlatformBankAccount { get; set; }
        #endregion

        #region Links
        BankAccount @userBankAccount;
        [BsonIgnore]
        public BankAccount UserBankAccount
        {
            get
            {
                if (@userBankAccount == null)
                    @userBankAccount = Core.DataContext.Store.GetById<BankAccount>(_UserBankAccount);
                return @userBankAccount;
            }
        }

        BankAccount @platformBankAccount;
        [BsonIgnore]
        public BankAccount PlatformBankAccount
        {
            get
            {
                if (@platformBankAccount == null)
                    @platformBankAccount = Core.DataContext.Store.GetById<BankAccount>(_PlatformBankAccount);
                return @platformBankAccount;
            }
        }
        #endregion

        #endregion
    }

    */
    public class UserTransactionRequestMetadata
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}
