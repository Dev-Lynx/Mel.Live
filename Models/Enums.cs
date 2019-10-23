using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentType
    {
        Deposit,
        Withdrawal,
        Credit,
        Debit,
        Direct
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChannelType
    {
        Platform = 1,
        Bank = 2,
        Paystack = 3
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        Pending = 0,
        Approved = 1,
        Declined = 2,
        Cancelled = 3
    }
}
