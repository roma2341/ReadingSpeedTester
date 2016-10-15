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
       private DateTime creationTime;
        private ReadingActivity readingActivity = null;
        /// <summary>
        /// Demanded to store "red" text block, which is skeeped by user (he don't read it)
        /// </summary>
        private int lastIndex = -1;

       public int GetLastIndex()
       {
           return lastIndex;
       }


       public int GetFragmentsLength()
       {
           return text.Length;
       }

       private string textFragmentToString(TextFragment fragment)
       {
          int start = fragment.getStartIndex();
           int length = fragment.getLength();
           string result = text.Substring(start, length);
           return result;
       }

       public DateTime getCreationTime()
       {
           return creationTime;
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
            readingActivity = new ReadingActivity();
            creationTime = DateTime.Now;
        }

       public void toggleReadingActivity()
       {
            readingActivity.toggleActivity();
       }
        /// <summary>
        /// Return skeeped text fragment or null if new text fragment is started
        /// </summary>
        /// <returns></returns>


        public TextFragment createFirstFragment(int endIndex)
        {
          return  createNewFragmentAtTimeTillNow(endIndex,creationTime);
        }

       public TextFragment addPerceivedFragment(int endIndex)
       {
            readingActivity = new ReadingActivity();
          TextFragment perceivedFragment = createFragment(endIndex);
            perceivedFragment.setPerceivity(true);
            perceivedFragment.setActivity(readingActivity);
           fragments.Add(perceivedFragment);
           return perceivedFragment;
       }
        public TextFragment addNotPerceivedFragment(int endIndex)
        {
            readingActivity = new ReadingActivity();
            TextFragment notPerceivedFragment = createFragment(endIndex);
            notPerceivedFragment.setPerceivity(false);
            notPerceivedFragment.setActivity(readingActivity);
            fragments.Add(notPerceivedFragment);
            return notPerceivedFragment;
        }

        public TextFragment createFragment(int endIndex)
       {
           TextFragment fragment;
           if (fragments.Count < 1)
           {

               fragment = createFirstFragment(endIndex);
           }
           else
           {
               fragment = createNewFragmentAtTimeTillNow(endIndex, fragments[fragments.Count-1].getEndTime());
           }
           return fragment;
       }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="createTextFragmentIfNotSelectedTextBefore">If true - create new fragment between old fragment and new</param>
        /// <returns></returns>
        public TextFragment createNewFragmentAtTimeTillNow(int endIndex,DateTime? time)
        {
            int startIndex = lastIndex + 1;
            //saveExtraTextFragment();
 
            int textlength = text.Length;
            if (startIndex >= textlength || startIndex < 0)
            {
             throw new IndexOutOfRangeException(string.Format("index {0} out of range [0;{1})", startIndex, textlength));
            }
            TextFragment currentTextFragment = TextFragment.beginFragment(startIndex, time);
            currentTextFragment.finish(endIndex);
            updateLastIndex(endIndex);
            Console.WriteLine("New text fragment successfully started at index:"+ startIndex);
            return currentTextFragment;
        }

        private void updateLastIndex(int index)
        {
            Console.WriteLine("Last index updated to "+ index);
            this.lastIndex = index;
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

       public List<Tuple<String, String>> getStatisticPerFragment()
       {
           List<Tuple<String, String>> resultList = new List<Tuple<string, string>>();
               
            for (int i = 0; i < fragments.Count; i++)
            {
                int fragI = i + 1;
                TextFragment fragment = fragments[i];
                    TextFragment previousFragment =  fragments[i - 1];
                   // if(!previousFragment.getPerceivity())//не сприйнятий елемент по часу закривається одразу ж після початку, бо
                    //він створюється після першого ж кліку, який відкриває елемент, що читається
                    //timeDeltaMs = TimeUtils.deltaBetweenTwoDatesMs(fragment.getStartTime(), previousFragment.getStartTime());
               long fragmentIdleTimeMs = fragment.getActivity().getTotalPauseTimeMs();
                long fragmentActiveTimeMs = fragment.getActivity().getTotalActiveTimeMs();
                // else timeDeltaMs = TimeUtils.deltaBetweenTwoDatesMs(fragment.getStartTime(), previousFragment.getEndTime());//TODO i think this work
                resultList.Add(new Tuple<String, String>("Q" + fragI, fragment.getLength().ToString() + "символів"));
                resultList.Add(new Tuple<String, String>("T" + fragI, fragmentActiveTimeMs.ToString() + "мс"));
                resultList.Add(new Tuple<string, string>("Тз" + fragI, fragmentIdleTimeMs.ToString()+"мс"));
            }
           return resultList;
       }

    }
}
