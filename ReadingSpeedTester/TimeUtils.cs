using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingSpeedTester
{
    class TimeUtils
    {
        static long dateTimeToMS(DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
        }
    }
}
