using MongoDB.Bson;
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
    [CollectionName("PaymentChannels")]
    public class PaymentChannel : IDocument<int>
    {
        #region Properties
        public int Id { get; set; }
        public int Version { get; set; }

        public string Name { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }

        public decimal ConversionRate { get; set; }

        [BsonRepresentation(BsonType.String)]
        public ChannelType Type { get; set; }

        public bool IsActive { get; set; } = true;
        #endregion
    }
}
