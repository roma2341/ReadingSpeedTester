using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

       public string textFragmentToString(TextFragment fragment)
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
               if (fragment.getPerceivity()) //if text fragment is includeReaded
                   strBuilder.Append(textFragmentToString(fragment));
           }
           return strBuilder.ToString();
       }

       public string[] getFragmentsStrings()
       {
           string[] result = new string[fragments.Count];
           int i = 0;
           foreach (var fragment in fragments)
           {
                result[i] = textFragmentToString(fragment);
                Console.WriteLine("::"+fragment.getStartIndex()+":"+fragment.getLength());
               i++;
           }
           return result;
       }

       public string getNonPerceivedText()
       {
            StringBuilder strBuilder = new StringBuilder();
            foreach (TextFragment fragment in fragments)
            {
                if (!fragment.getPerceivity()) //if text fragment is includeReaded
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
                    readingTimeMs += fragment.getActivity().getTotalActiveTimeMs();
                    readedCharacters += fragment.getLength();
                    readedWords += TimeUtils.WordCounting.CountWords1(textFragmentToString(fragment));
                }
                else
                {
                    idleTime += fragment.getActivity().getTotalPauseTimeMs();
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

       public bool toggleReadingActivity()
       {
            return readingActivity.toggleActivity();
       }

       public void resumeReadingActivity()
       {
            readingActivity.activate();
       }
        /// <summary>
        /// Return skeeped text fragment or null if new text fragment is started
        /// </summary>
        /// <returns></returns>


       public TextFragment addPerceivedFragment(int endIndex)
       {
          
          TextFragment perceivedFragment = createFragment(endIndex);
            perceivedFragment.setPerceivity(true);
            readingActivity.pause();
            perceivedFragment.setActivity(readingActivity);
            fragments.Add(perceivedFragment);
            readingActivity = new ReadingActivity();
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
            int startIndex = lastIndex + 1;
            //saveExtraTextFragment();

            int textlength = text.Length;
            if (startIndex >= textlength || startIndex < 0)
            {
                throw new IndexOutOfRangeException(string.Format("index {0} out of range [0;{1})", startIndex, textlength));
            }
            TextFragment currentTextFragment = TextFragment.beginFragment(startIndex);
            currentTextFragment.finish(endIndex);
            updateLastIndex(endIndex);
            Console.WriteLine("New text fragment successfully started at index:" + startIndex);
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
           // container.text=container.text.Trim();
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
        public Dictionary<String,Object> generateStatisticChartData(ChartStatisticCriteria criteria)
          //baseline,average
        {
            Dictionary<String, Object> result = new Dictionary<string, object>();
            List<Point> points = new List<Point>();
            List<TextFragment> fragments = getFragments();
            long totalTime = 0;
            int totalLength = 0;
            foreach (TextFragment fragment in fragments)
            {
                int length = 0;
                long time = 0;
                if ((criteria.IncludeReaded && fragment.getPerceivity()) || (criteria.IncludeSkipped && !fragment.getPerceivity()))
                {
                    time += fragment.getActivity().getTotalActiveTimeMs();
                    if (criteria.IncludePauseTime) time += fragment.getActivity().getTotalPauseTimeMs();
                    if (criteria.WordsMeasure)
                    {
                        int wordsCount = TimeUtils.WordCounting.CountWords1(textFragmentToString(fragment));
                        length += wordsCount;
                    }
                    else
                    length += fragment.getLength();
                }

                totalTime += time;
                totalLength += length;
                if (criteria.Accumulating)
                    points.Add(new Point(TimeUtils.msToS(totalTime), totalLength));
                else
                {
                    if (criteria.Average)
                    {
                        double avgValue = time == 0 ? 0 : length/ TimeUtils.msToS(time);
                        points.Add(new Point(TimeUtils.msToS(totalTime), avgValue));
                    }
                    else
                        points.Add(new Point(TimeUtils.msToS(totalTime), length));
                }
            }
            if (criteria.BaseLine)
            {
                List<Point> extraPoints = new List<Point>();
                double totalTimeMs = TimeUtils.msToS(totalTime);
                double mediana = totalTimeMs==0 ? 0 : totalLength / TimeUtils.msToS(totalTime);
                extraPoints.Add(new Point(0,mediana));
                extraPoints.Add(new Point(TimeUtils.msToS(totalTime), mediana));
                result["extraPoints"] = extraPoints;
            }
            result["points"] = points;
           
            return result;
        }

    }
}
