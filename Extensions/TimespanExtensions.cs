using Mel.Live.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Extensions
{
    public static class TimespanExtensions
    {
        public static string ToHumanReadableString(this TimeSpan t)
        {
            if (t.TotalSeconds <= 1)
            {
                return $@"{t:s\.ff} seconds";
            }
            if (t.TotalMinutes <= 1)
            {
                return $@"{t:%s} seconds";
            }
            if (t.TotalHours <= 1)
            {
                return $@"{t:%m} minutes";
            }
            if (t.TotalDays <= 1)
            {
                return $@"{t:%h} hours";
            }

            return $@"{t:%d} days";
        }

        public static string ToHumanReadableString(this TimeRange range)
        {
            TimeSpan from = range.Start;
            TimeSpan to = range.End;

            if (to.TotalSeconds == 0 && from.TotalSeconds == 0) return "Instant";

            if (to.TotalSeconds <= 1) return $@"{from.TotalSeconds} - {to.TotalSeconds} seconds";
            if (to.TotalMinutes <= 1) return $@"{from.TotalSeconds} - {to.TotalSeconds} seconds";
            if (to.TotalHours <= 1) return $@"{from.TotalMinutes} - {to.TotalMinutes} minutes";
            if (to.TotalDays <= 1) return $@"{from.TotalHours} - {to.TotalHours} hours";

            return $@"{from.TotalDays} - {to.TotalDays} days";
        }

        public static DateRange ToLocalTime(this DateRange range)
        {
            return new DateRange(range.Start.ToLocalTime(), range.End.ToLocalTime());
        }
    }
}
