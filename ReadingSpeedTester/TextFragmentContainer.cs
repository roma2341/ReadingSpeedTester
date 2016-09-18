using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadingSpeedTester
{
   public class TextFragmentContainer
    {
        private String text;
        private List<TextFragment> fragments;
        private TextFragment currentTextFragment;
        /// <summary>
        /// Demanded to store "red" text block, which is skeeped by user (he don't read it)
        /// </summary>
       private TextFragment extraTextFragment;
        private int lastIndex = -1;

       private string textFragmentToString(TextFragment fragment)
       {
          int start = fragment.getStartIndex();
           int length = fragment.getLength();
           string result = text.Substring(start, length);
           return result;
       }

       public string getPerceivedText()
       {
           StringBuilder strBuilder = new StringBuilder();
           foreach (TextFragment fragment in fragments)
           {
               if (fragment.getPerceivity()) //if text fragment is readed
                   strBuilder.Append(textFragmentToString(fragment));
           }
           return strBuilder.ToString();
       }

       public string getNonPerceivedText()
       {
            StringBuilder strBuilder = new StringBuilder();
            foreach (TextFragment fragment in fragments)
            {
                if (!fragment.getPerceivity()) //if text fragment is readed
                    strBuilder.Append(textFragmentToString(fragment));
            }
            return strBuilder.ToString();
        }

       public TextFragmentContainerStatistic getStatistic()
       {
           TextFragmentContainerStatistic statistic;
            long readingTimeMs = 0;
           long idleTime = 0;
           int readedCharacters = 0,readedWords = 0;
           int ignoredCharacters = 0,ignoredWords = 0;
            foreach (TextFragment fragment in fragments)
            {
                if (fragment.getPerceivity())
                {
                    readingTimeMs += fragment.getTimeDeltaMS();
                    readedCharacters += fragment.getLength();
                    readedWords += TimeUtils.WordCounting.CountWords1(textFragmentToString(fragment));
                }
                else
                {
                    idleTime += fragment.getTimeDeltaMS();
                    ignoredCharacters += fragment.getLength();
                    ignoredWords += TimeUtils.WordCounting.CountWords1(textFragmentToString(fragment));
                }
            }
           long totalTimeMs = readingTimeMs + idleTime;
           int totalTimeSeconds = (int)(totalTimeMs / 1000);
           int readingTimeSeconds = (int)(readingTimeMs / 1000);
            statistic.ReadedCharacters = readedCharacters;
           statistic.IgnoredCharacters = ignoredCharacters;
           statistic.ReadedWords = readedWords;
           statistic.IgnoredWords = ignoredWords;
           statistic.ReadingTime = readingTimeMs;
           statistic.IdleTime = idleTime;
           if (totalTimeSeconds != 0)
           {
               statistic.AverageReadingSpeedCharactersPerMinute = (int) (readedCharacters/totalTimeSeconds)*60;
               statistic.AverageReadingSpeedWordsPerMinute = (int) (readedWords/totalTimeSeconds)*60;
           }
           else
           {
               statistic.AverageReadingSpeedCharactersPerMinute = 0;
                statistic.AverageReadingSpeedWordsPerMinute = 0;
           }
           if (readingTimeSeconds != 0)
           {
               statistic.AverageActiveReadingSpeedCharactersPerMinute = (int) (readedCharacters/readingTimeSeconds)*60;
                statistic.AverageActiveReadingSpeedWordsPerMinute = (int)(readedWords / readingTimeSeconds) * 60;
            }
           else
           {
               statistic.AverageActiveReadingSpeedCharactersPerMinute = 0;
               statistic.AverageActiveReadingSpeedWordsPerMinute = 0;
           }
          
            return statistic;
       }


        private TextFragmentContainer()
        {
            fragments = new List<TextFragment>();
        }
        /// <summary>
        /// Return skeeped text fragment or null if new text fragment is started
        /// </summary>
        /// <returns></returns>
       public TextFragment getExtraTextFragment()
       {
           return extraTextFragment;
       }

        public TextFragment startOrFinishFragment(int index)
        {
            if (currentTextFragment == null)
            return startNewFragment(index);
            else return finishCurrentFragment(index);
        }

        private TextFragment fillNotSelectedTextByFragment(int newStartIndex)
        {
            int startIndex = lastIndex+1;
            int endIndex = newStartIndex - 1;
            int textlength = text.Length;
            if (newStartIndex < 0 || newStartIndex>=textlength)
            {
                throw new IndexOutOfRangeException(string.Format("index {0} out of range [0;{1})", newStartIndex, textlength));
            }
            if (startIndex >= textlength || startIndex < 0)
            {
                return null;
            }
            if (endIndex >= textlength || endIndex < 0)
            {
                return null;
            }
            if (startIndex >= endIndex) return null;
            DateTime time = DateTime.Now;
            if (fragments.Count>0) time = fragments.Last().getEndTime();
            startNewFragmentAtTime(startIndex, time, false);
            TextFragment finishedFragment = currentTextFragment;
            finishCurrentFragment(endIndex,false);
            return finishedFragment;


        }

       public TextFragment startNewFragment(int startIndex, bool createTextFragmentIfNotSeelctedTextBefore = true)
       {
           return startNewFragmentAtTime(startIndex, null, createTextFragmentIfNotSeelctedTextBefore);     
       }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="createTextFragmentIfNotSelectedTextBefore">If true - create new fragment between old fragment and new</param>
        /// <returns></returns>
        public TextFragment startNewFragmentAtTime(int startIndex,DateTime? time,bool createTextFragmentIfNotSelectedTextBefore = true)
        {
            TextFragment tempExtraFragment = null;
            if (createTextFragmentIfNotSelectedTextBefore) tempExtraFragment = fillNotSelectedTextByFragment(startIndex);
            extraTextFragment = tempExtraFragment;
            saveExtraTextFragment();
            if (startIndex <= lastIndex)
            {
                Console.WriteLine("Error, start index must be greater than last selection index");
                return null;
            }
            int textlength = text.Length;
            if (startIndex >= textlength || startIndex < 0)
            {
             throw new IndexOutOfRangeException(string.Format("index {0} out of range [0;{1})",startIndex, textlength));
            }
            currentTextFragment = TextFragment.beginFragment(startIndex,time);
            updateLastIndex(startIndex);
            Console.WriteLine("New text fragment successfully started at index:"+startIndex);
            return currentTextFragment;
        }

        public TextFragment finishCurrentFragment(int endIndex, bool isPerceived = true)
        {
            currentTextFragment.setPerceivity(isPerceived);
            if (endIndex <= lastIndex)
            {
                Console.WriteLine("Error, start index must be greater than last selection index");
                return null;
            }
            int textlength = text.Length;
            if (endIndex >= textlength || endIndex < 0)
            {
                throw new IndexOutOfRangeException(string.Format("index {0} out of range [0;{1})", endIndex, textlength));
            }
            currentTextFragment.finish(endIndex);
            saveCurrentTextFragment();
            TextFragment finishedFragment = currentTextFragment;
            resetCurrentTextFragment();
            updateLastIndex(endIndex);
            Console.WriteLine("New text fragment successfully finished at index:" + endIndex);
            return finishedFragment;
        }

        private void updateLastIndex(int index)
        {
            this.lastIndex = index;
        }

       private void saveExtraTextFragment()
       {
            if (extraTextFragment!=null)
           fragments.Add(extraTextFragment);
       }
        private void saveCurrentTextFragment()
        {
            fragments.Add(currentTextFragment);
        }
        private void resetCurrentTextFragment()
        {
            currentTextFragment = null;
        }

        public static TextFragmentContainer fromText(String text)
        {
            TextFragmentContainer container = new TextFragmentContainer();
            container.text = text;
            return container;
        }

        public List<TextFragment> getFragments()
        {
            return fragments;
        }

    }
}
