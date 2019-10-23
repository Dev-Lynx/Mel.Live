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
    [CollectionName("Platforms")]
    public class Platform : DocumentBase
    {
        #region Properties
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public List<int> LeafNodes { get; set; } = new List<int>();
        public Guid _DisplayPicture { get; set; } = Guid.Empty;

        #region Collections
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<PlatformRule> Rules { get; set; } = new List<PlatformRule>();

        public List<ResourceBase> Resources { get; set; } = new List<ResourceBase>();
        #endregion

        #region References
        public List<Guid> _Products { get; set; } = new List<Guid>();
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

        IEnumerable<Product> @products = Enumerable.Empty<Product>();
        [BsonIgnore]
        public IEnumerable<Product> Products
        {
            get
            {
                if (@products.Count() != _Products.Count())
                    @products = Core.DataContext.Get<Product>(_Products);
                return @products;
            }
        }
        #endregion

        #endregion

        #region Methods
        public override Task InitializeAsync()
        {
            Active = false;
            LeafNodes.Add(1);

            return base.InitializeAsync();
        }

        public void Update(Platform platform)
        {
            Name = platform.Name;
            Description = platform.Description;
        }
        #endregion
    }
}
