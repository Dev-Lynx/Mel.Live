using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Options
{
    public class CloudinaryOptions
    {
        #region Properties

        public const string ConfigKey = "Cloudinary";

        public string Key { get; set; }
        public string Secret { get; set; }
        public string CloudName { get; set; }
        
        #endregion
    }
}
