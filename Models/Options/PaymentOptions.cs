using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Options
{
    public class PaystackOptions
    {
        public const string ConfigKey = "Paystack";
        public const string TransactionEvent = "charge.success";

        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
    }
}
