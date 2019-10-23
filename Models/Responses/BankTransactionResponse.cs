using Mel.Live.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Responses
{
    public class BankTransactionRequest
    {
        public User User { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public BankTransactionMetadata Meta { get; set; }
    }
}
