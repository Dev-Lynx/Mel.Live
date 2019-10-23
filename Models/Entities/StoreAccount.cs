using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [Owned]
    public class StoreAccount
    {
        #region Properties
        public List<Location> AddressBook { get; set; } = new List<Location>();

        #region References
        public List<Guid> _Favorites { get; set; } = new List<Guid>();
        public List<CartProduct> _Cart { get; set; } = new List<CartProduct>();
        #endregion

        #region Links
        IEnumerable<Product> @favorites = Enumerable.Empty<Product>();
        [BsonIgnore]
        public IEnumerable<Product> Favorites
        {
            get
            {
                if (@favorites.LongCount() != _Favorites.LongCount())
                    @favorites = Core.DataContext.GetAsync<Product>(_Favorites).Result;
                return @favorites;
            }
        }
        #endregion

        #endregion
    }

    public class CartProduct
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }

    [Owned]
    public class Location : Document
    {
        #region Properties
        public string City { get; set; }
        public string State { get; set; }
        public string Address { get; set; }

        public string IsDefault { get; set; }
        #endregion
    }
}
