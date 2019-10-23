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
    [CollectionName("UserRanks")]
    public class UserRank : IDocument<int>
    {
        #region Properties
        public int Id { get; set; }
        public int Version { get; set; }
        public string Title { get; set; }
        public string Badge { get; set; }
        #endregion
    }
}
