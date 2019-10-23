using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities.BackOffice
{
    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("Companies")]
    public class Company : DocumentBase
    {
        #region Properties
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid _Logo { get; set; } = Guid.Empty;

        #region Collections
        public List<string> PhoneNumbers { get; set; } = new List<string>();
        public List<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
        public List<ResourceBase> Resources { get; set; } = new List<ResourceBase>();
        #endregion

        #region References
        public List<Guid> _Platforms { get; set; }
        #endregion

        #region Links
        Photo @logo;
        [BsonIgnore]
        public Photo Logo
        {
            get
            {
                if (@logo == null)
                    @logo = Resources.FirstOrDefault(r => r.Id == _Logo) as Photo;

                return @logo;
            }
        }

        IEnumerable<Platform> @platforms = Enumerable.Empty<Platform>();
        public IEnumerable<Platform> Platforms
        {
            get
            {
                if (@platforms.Count() != _Platforms.Count())
                    @platforms = Core.DataContext.Get<Platform>(_Platforms);
                return @platforms;
            }
        }
        #endregion

        #endregion

        #region Methods
        public void Update(Company company)
        {
            Name = company.Name;
            Email = company.Email;
            PhoneNumbers = company.PhoneNumbers;
        }
        #endregion
    }
}
