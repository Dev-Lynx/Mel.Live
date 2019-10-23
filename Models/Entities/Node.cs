using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [ComplexType]
    [BsonIgnoreExtraElements]
    [CollectionName("Nodes")]
    public class Node
    {
        #region Properties
        public string Name { get; set; }
        public int Index { get; set; }
        public DateTime Created { get; set; }

        public int LeftBranch { get; set; }
        public int RightBranch { get; set; }

        public int Parent => Index / 2;
        public int Left => Index * 2;
        public int Right => Left + 1;
        public int Total => Left + Right;

        #region References
        public Guid _User { get; set; }
        #endregion

        #region Links
        User @user;
        [BsonIgnore]
        public User User
        {
            get
            {
                if (@user == null)
                    @user = Core.DataContext.Store.GetById<User>(_User);
                return @user;
            }
        }
        #endregion

        #endregion
    }
}
