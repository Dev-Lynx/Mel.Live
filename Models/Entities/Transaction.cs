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
    [BsonDiscriminator(Required = true, RootClass = true)]
    [CollectionName("Transactions")]
    public class Transaction : DocumentBase
    {
        #region Properties
        public decimal Amount { get; set; }
        public Status Status { get; set; }
        public PaymentType PaymentType { get; set; }

        public UserTransactionMetadata Meta { get; set; } = new UserTransactionMetadata();

        #region References
        public int _PaymentChannel { get; set; }
        #endregion

        #region Collections
        public List<ResourceBase> Resources { get; set; } = new List<ResourceBase>();
        #endregion

        #region Links
        User @user;
        [BsonIgnore]
        public User User
        {
            get
            {
                if (@user == null)
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

    #region Metadata
    public class UserTransactionMetadata
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CompletionDate { get; set; }
    }

    public class CreditTransactionMetadata : UserTransactionMetadata
    {
        public int Node { get; set; }
        public Guid Platform { get; set; }
    }

    public class PurchaseTransactionMetadata : UserTransactionMetadata
    {
        public string OrderId { get; set; }
    }

    public class BankTransactionMetadata : UserTransactionMetadata
    {
        public Guid BankAccountId { get; set; }
        public Guid CompanyAccountId { get; set; }
    }
    #endregion
}
