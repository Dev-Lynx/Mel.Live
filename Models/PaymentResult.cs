using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models
{
    public enum PaymentStatus
    {
        Success, Redirected, Pending, Failed
    }

    public class PaymentResult
    {
        public PaymentStatus Status { get; set; }
        public string Message { get; set; }
    }
}
