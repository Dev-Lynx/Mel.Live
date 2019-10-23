using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    public class DocumentBase : Document
    {
        public virtual Task InitializeAsync()
        {
            AddedAtUtc = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}
