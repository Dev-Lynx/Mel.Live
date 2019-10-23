using Mel.Live.Extensions;
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
    [CollectionName("Products")]
    [BsonKnownTypes(typeof(ProductPackage))]
    [BsonDiscriminator(RootClass = true, Required = true)]
    public class Product : DocumentBase
    {
        #region Properties
        public decimal Price { get; set; }

        /// <summary>
        /// Unique name of product. Must not contain 
        /// spaces or other characters that are not 
        /// compatible with links.
        /// </summary>
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        // public string SKU { get; set; }
        public List<ResourceBase> Resources { get; set; } = new List<ResourceBase>();
        public List<string> Tags { get; set; } = new List<string>();
        public bool IsActive { get; set; } = true;

        #region References
        public Guid _DisplayPicture { get; set; }
        #endregion

        #region Links
        Photo @displayPicture;
        [BsonIgnore]
        public Photo DisplayPicture
        {
            get
            {
                if (@displayPicture == null)
                    @displayPicture = Resources.FirstOrDefault(r => r.Id == _DisplayPicture) as Photo;

                return @displayPicture;
            }
        }
        #endregion

        #endregion

        #region Methods
        public virtual void Update(Product product)
        {
            Name = product.Name;
            Price = product.Price;
            IsActive = product.IsActive;
            Description = product.Description;
            Tags = product.Tags;
        }

        public override Task InitializeAsync()
        {
            base.InitializeAsync();

            if (string.IsNullOrWhiteSpace(Name))
                Name = Title;

            Name = Name.UrlFriendly();
            return Task.CompletedTask;
        }
        #endregion
    }
}
