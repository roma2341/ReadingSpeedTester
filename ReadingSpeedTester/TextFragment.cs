using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingSpeedTester
{
    public class TextFragment
    {
        private int startIndex;
        private int endIndex;
        private DateTime startTime;
        private DateTime endTime;
        private bool isPerceived = false;//Чи прочитаний(сприйнятий)
        private bool finished = false;

        public DateTime getStartTime()
        {
            return startTime;
        }

        public DateTime getEndTime()
        {
            return endTime;
        }
        public bool isFinished()
        {
            return finished;
        }

        public void setFinished(bool finished)
        {
            this.finished = finished;
        }
        private TextFragment()
        {
            
        }

        public void setPerceivity(bool perceivity)
        {
            this.isPerceived = perceivity;
        }

        public bool getPerceivity()
        {
            return isPerceived;
        }

        public static TextFragment beginFragment(int start,DateTime? time = null)
        {
            TextFragment textFragment = new TextFragment();
            textFragment.startIndex = start;
            textFragment.startTime = null==time ? DateTime.Now : (DateTime)time;
            return textFragment;
        }

        public void finish(int index)
        {
            endIndex = index;
            endTime = DateTime.Now;
            this.finished = true;
        }

        public int getStartIndex()
        {
            return startIndex;
        }

        public int getLength()
        {
            return endIndex - startIndex;
        }

        public long getTimeDeltaMS()
        {
            return (long)(endTime.Subtract(startTime).TotalMilliseconds);
        }

        public long getTimeDifference(DateTime time)
        {
            return (long) time.Subtract(startTime).TotalMilliseconds;


        }

    }
}
