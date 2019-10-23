using Microsoft.EntityFrameworkCore;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.Entities
{
    [Owned]
    public class BankAccount : Document
    {
        #region Properties
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; }
        #endregion
    }
}
