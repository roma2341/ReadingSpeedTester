using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingSpeedTester
{
   public struct TextFragmentContainerStatistic
    {
       public long ReadingTime, IdleTime;
        public int AverageReadingSpeedCharactersPerMinute, AverageReadingSpeedWordsPerMinute, 
            AverageActiveReadingSpeedCharactersPerMinute, AverageActiveReadingSpeedWordsPerMinute,
            ReadedCharacters, IgnoredCharacters,ReadedWords,IgnoredWords;

    }
}
