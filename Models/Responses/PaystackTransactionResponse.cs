using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Responses
{
    public class PaystackTransactionResponse
    {
        public string Event { get; set; }
        public PaystackTransactionData Data { get; set; }
    }

    public class PaystackTransactionData
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public Dictionary<string, object> Metadata { get; set; }

        public PaystackPaymentAuthorization Authorization { get; set; }
    }

    public class PaystackPaymentAuthorization
    {
        public string AuthorizationCode { get; set; }
        public string Bank { get; set; }
        public string Brand { get; set; }
        public bool Reusable { get; set; }
        public string Signature { get; set; }
    }
}
