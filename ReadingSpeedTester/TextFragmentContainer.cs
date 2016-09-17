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
            startNewFragment(startIndex,false);
            TextFragment finishedFragment = currentTextFragment;
            finishCurrentFragment(endIndex,false);
            return finishedFragment;


        }
        public TextFragment startNewFragment(int startIndex,bool createTextFragmentIfNotSelectedTextBefore = true)
        {
            TextFragment tempExtraFragment = null;
            if (createTextFragmentIfNotSelectedTextBefore) tempExtraFragment = fillNotSelectedTextByFragment(startIndex);
            extraTextFragment = tempExtraFragment;
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
            currentTextFragment = TextFragment.beginFragment(startIndex);
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
