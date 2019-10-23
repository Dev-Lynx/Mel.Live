using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Options
{
    public class MongoDBOptions
    {
        public const string Key = "MongoDB";

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
