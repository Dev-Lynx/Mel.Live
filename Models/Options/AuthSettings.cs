using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mel.Live.Models.Options
{
    public class AuthSettings
    {
        public const string ConfigKey = "Authentication";

        public string Key { get; set; } = string.Empty;

        SymmetricSecurityKey symmetricKey = null;
        public SymmetricSecurityKey SymmetricKey
        {
            get
            {
                if (symmetricKey != null)
                    return symmetricKey;

                byte[] buffer = new byte[32];
                try
                {
                    buffer = Encoding.ASCII.GetBytes(Key);
                    symmetricKey = new SymmetricSecurityKey(buffer);
                }
                catch (Exception ex)
                {
                    Core.Log.Error($"Failed to attain bytes from authentication key.\n{ex}");
                }

                return symmetricKey;
            }
        }
    }
}
