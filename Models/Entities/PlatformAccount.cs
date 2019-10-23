using Mel.Live.Models.Entities.BackOffice;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [Owned]
    public class PlatformAccount
    {
        #region Properties

        #region References
        public Guid _Platform { get; set; } = Guid.Empty;
        public List<int> _Nodes { get; set; } = new List<int>();
        #endregion

        #region Links
        Platform @platform;
        [BsonIgnore]
        public Platform Platform
        {
            get
            {
                if (@platform == null)
                    @platform = Core.DataContext.Store.GetOne<Platform>(p => p.Id == _Platform);
                return @platform;
            }
        }

        IEnumerable<Node> @nodes = Enumerable.Empty<Node>();
        [BsonIgnore]
        public IEnumerable<Node> Nodes
        {
            get
            {
                if (@nodes.LongCount() != _Nodes.LongCount())
                    @nodes = Platform.Nodes.Where(n => _Nodes.Contains(n.Index));

                return @nodes;
            }
        }
        #endregion

        #endregion

    }
}
