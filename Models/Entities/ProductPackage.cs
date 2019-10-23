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
    [CollectionName("Users")]
    public class ProductPackage : Product
    {
        #region Properties
        public decimal DiscountPercentage { get; set; }

        #region References
        public List<Guid> _Products { get; set; } = new List<Guid>();
        #endregion

        #region Links
        IEnumerable<Product> @products = Enumerable.Empty<Product>();
        public IEnumerable<Product> Products
        {
            get
            {
                if (@products.LongCount() != _Products.LongCount())
                    @products = Core.DataContext.GetAsync<Product>(_Products).Result;
                return @products;
            }
        }
        #endregion

        #endregion

        #region Methods

        public override void Update(Product product)
        {
            base.Update(product);

            if (product is ProductPackage package)
                _Products = package._Products;
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            CalculatePrice();
        }

        void CalculatePrice()
        {
            decimal price = 0M;
            foreach (var product in Products)
                price += product.Price;

            Price = price - ((price * DiscountPercentage) / 100);
        }
        #endregion
    }
}
