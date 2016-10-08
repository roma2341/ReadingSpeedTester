using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadingSpeedTester
{
    public class TimeUtils
    {
        public static long dateTimeToMS(DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                ).TotalMilliseconds;
        }

        public static long deltaBetweenTwoDatesMs(DateTime date1, DateTime date2)
        {
            long delta = dateTimeToMS(date1) - dateTimeToMS(date2);
            return delta;
        }

        public static string formatTimeToHumanReadableForm(long ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
            return answer;
        }
        /// <summary>
        /// Contains methods for counting words.
        /// </summary>
        public static class WordCounting
        {
            /// <summary>
            /// Count words with Regex. It's Better in every way except perhaps perfomance
            /// </summary>
            public static int CountWords1(string s)
            {
                MatchCollection collection = Regex.Matches(s, @"[\S]+");
                return collection.Count;
            }

            /// <summary>
            /// Count word with loop and character tests.
            /// </summary>
            public static int CountWords2(string s)
            {
                int c = 0;
                for (int i = 1; i < s.Length; i++)
                {
                    if (char.IsWhiteSpace(s[i - 1]) == true)
                    {
                        if (char.IsLetterOrDigit(s[i]) == true ||
                            char.IsPunctuation(s[i]))
                        {
                            c++;
                        }
                    }
                }
                if (s.Length > 2)
                {
                    c++;
                }
                return c;
            }
        }

    }
}
