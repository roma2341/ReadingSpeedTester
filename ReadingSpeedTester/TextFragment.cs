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
        private bool isPerceived = false;//Чи прочитаний(сприйнятий)
        private bool finished = false;
        private ReadingActivity readingActivity;


        public void setActivity(ReadingActivity activity)
        {
            this.readingActivity = activity;
        }

        public ReadingActivity getActivity()
        {
            return readingActivity;
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
        public void finish(int index)
        {
            endIndex = index;
            this.finished = true;
        }

        public int getStartIndex()
        {
            return startIndex;
        }

        public int getLength()
        {
            return endIndex - startIndex+1;
        }
        public static TextFragment beginFragment(int start)
        {
            TextFragment textFragment = new TextFragment();
            textFragment.startIndex = start;
            return textFragment;
        }

    }
}
