using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Models.ViewModels
{
    public class DateSearchViewModel
    {
        public string SearchQuery { get; set; }
        public DateTime Start { get; set; } = DateRange.AllTime.Start;
        public DateTime End { get; set; } = DateRange.AllTime.End;
    }

    public class SearchableSieveModel : SieveModel
    {
        public string SearchQuery { get; set; }
    }

    public class DateRangeSieveModel : SearchableSieveModel
    {
        public DateTime Start { get; set; } = DateRange.AllTime.Start;
        public DateTime End { get; set; } = DateRange.AllTime.End;
    }
}
