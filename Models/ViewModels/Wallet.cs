using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels
{
    public class TransactionViewModel
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public PaymentType Type { get; set; }
    }

    public class PaymentChannelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelType Type { get; set; }
    }
    
    public class DepositViewModel
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int Channel { get; set; }

        public DateTime TransactionDate { get; set; }
        public Guid BankAccountId { get; set; }
        public Guid CompanyBankAccountId { get; set; }
        public string Description { get; set; }
    }

    public class WithdrawalViewModel
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public Guid BankAccountId { get; set; }
    }
}
