using AspNetCore.Identity.MongoDbCore.Models;
using Mel.Live.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    public enum UserRoles
    {
        Regular,
        Administrator,
    }

    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("Users")]
    public class User : MongoIdentityUser, IDocument
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string StateOfOrigin { get; set; }

        [BsonIgnoreIfDefault]
        public Referrer Referrer { get; set; }

        [BsonIgnoreIfDefault]
        public StoreAccount StoreAccount { get; set; }

        [NotMapped]
        public string FormattedPhoneNumber => Core.Phone.Format(Phone, PhoneNumberFormat.E164);
        [NotMapped]
        public string InternationalPhoneNumber => Core.Phone.Format(Phone, PhoneNumberFormat.INTERNATIONAL);

        #region Collections
        public List<PlatformAccount> PlatformAccounts { get; set; } = new List<PlatformAccount>();
        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public List<ResourceBase> Resources { get; set; } = new List<ResourceBase>();
        #endregion

        #region References
        [BsonIgnoreIfDefault]
        public int _Rank { get; set; }

        [BsonIgnoreIfDefault]
        public Guid _Wallet { get; set; }

        [BsonIgnoreIfDefault]
        public List<int> _Nodes { get; set; } = new List<int>();

        public Guid _DisplayPicture { get; set; }
        #endregion

        #region Links
        Wallet @wallet;
        [BsonIgnore]
        public Wallet Wallet
        {
            get
            {
                if (@wallet == null)
                    @wallet = Core.DataContext.Store.GetById<Wallet>(_Wallet);
                return @wallet;
            }
        }

        UserRank @rank;
        [BsonIgnore]
        public UserRank Rank
        {
            get
            {
                if (@rank == null)
                    @rank = Core.DataContext.Store.GetById<UserRank, int>(_Rank);
                return @rank;
            }
        }

        Photo @displayPicture;
        [BsonIgnore]
        public Photo DisplayPicture
        {
            get
            {
                if (@displayPicture == null)
                    @displayPicture = Resources.FirstOrDefault(p => p.Id == _DisplayPicture) as Photo;
                return @displayPicture;
            }
        }
        #endregion

        #region Internals
        PhoneNumber Phone
        {
            get
            {
                try
                {
                    var phone = Core.Phone.Parse(PhoneNumber, "NG");
                    return phone;
                }
                catch { return new PhoneNumber(); }
            }
        }
        #endregion

        #endregion

        #region Methods
        public async Task InitializeAsync()
        {
            @wallet = new Wallet(this);
            StoreAccount = new StoreAccount();
            _Rank = 1;

            await wallet.InitializeAsync();
            await Core.DataContext.Store.AddOneAsync(wallet);

            _Wallet = @wallet.Id;
            await Core.DataContext.Store.UpdateOneAsync(this);
        }
        #endregion
    }

    [Owned]
    public class Referrer
    {
        public Guid User { get; set; }
        public int Node { get; set; }
    }
}
