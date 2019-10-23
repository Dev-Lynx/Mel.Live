using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    #region Base
    [ComplexType]
    [BsonIgnoreExtraElements]
    [BsonKnownTypes(typeof(Photo))]
    [BsonDiscriminator(RootClass = true, Required = true)]
    public class ResourceBase : Document
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; } = true;
        public string Signature { get; set; }

        [BsonIgnoreIfDefault]
        public Guid _Reference { get; set; }
    }
    #endregion

    #region Photos
    public enum PhotoType
    {
        [Description("Misc")]
        Misc,
        [Description("DisplayPicture")]
        DisplayPicture,
    }

    public class Photo : ResourceBase
    {
        public PhotoType Type { get; set; }
    }
    #endregion

}
