﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace ReadingSpeedTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextFragmentContainer textFragmentcontainer;
        private ResultsWindow resultsWindow;
        private List<int> indexesOfPunctuationMarks;
        private int currentPunctuationMarkIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            rtbText.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(rtbText_MouseLeftDown),true);
            rtbText.AddHandler(FrameworkElement.MouseRightButtonDownEvent, new MouseButtonEventHandler(rtbText_MouseRightDown), true);
        }


        void createTextFragmentAndColorizeIt(int carretPosition)
        {
            if (carretPosition <= textFragmentcontainer.GetLastIndex()) return;
            TextFragment textFragmentAfterAction;
            if (carretPosition == null || textFragmentcontainer == null) return;
            textFragmentAfterAction = textFragmentcontainer.addPerceivedFragment((int)carretPosition);
            updateRtbTextFromTextFragment(textFragmentAfterAction);
            if (SessionFinishingPossible(carretPosition))
            {
                finishSession();
            }
        }
        void moveLastFragmentIndexAndColorizeIt(int moveToPosition)
        {
            if (moveToPosition <= textFragmentcontainer.GetLastIndex()) return;
            TextFragment textFragmentAfterAction;
            if (moveToPosition == null || textFragmentcontainer == null) return;
            textFragmentAfterAction = textFragmentcontainer.addNotPerceivedFragment((int)moveToPosition);
            updateRtbTextFromTextFragment(textFragmentAfterAction);
            if (SessionFinishingPossible(moveToPosition))
            {
                finishSession();
            }
        }

        private bool SessionFinishingPossible(int carretPosition)
        {
            return carretPosition >= textFragmentcontainer.GetFragmentsLength();
        }

         void rtbText_MouseLeftDown(object sender, MouseButtonEventArgs e)
         {
             var senderType = sender.GetType();
            int carretPosition = TextUtils.toCarretPosition(rtbText);
            Console.WriteLine("left click carret position:" + carretPosition);
            createTextFragmentAndColorizeIt(carretPosition);
            if (SessionFinishingPossible(carretPosition))
            {
                finishSession();
            }

        }
        void rtbText_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            //set text cursor position;
            RichTextBox box = (RichTextBox)sender;
            box.CaretPosition = box.GetPositionFromPoint(e.GetPosition(box), true);

            var senderType = sender.GetType();
            int carretPosition = TextUtils.toCarretPosition(rtbText);
            Console.WriteLine("right click carret position:"+carretPosition);
            moveLastFragmentIndexAndColorizeIt(carretPosition);
            if (carretPosition >= textFragmentcontainer.GetFragmentsLength())
            {
                finishSession();
            }

        }

        private void addPunctuationMarksFromString(String str)
        {
            Regex rx = new Regex(@"[,\.]");
            indexesOfPunctuationMarks = new List<int>();
            foreach (Match match in rx.Matches(str))
            {
                int i = match.Index;
                indexesOfPunctuationMarks.Add(i);
            }
        }
        private void btnLoad_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            String fileText = "";
            String textFilePath = browseFile();
            if (textFilePath != null)
                fileText = loadFileContent(textFilePath);
            TextUtils.setRichTextBoxValue(rtbText, fileText);
            addPunctuationMarksFromString(fileText);
            currentPunctuationMarkIndex = 0;

             textFragmentcontainer = TextFragmentContainer.fromText(TextUtils.textFromRichTextBox(rtbText));
        }
 
        private string loadFileContent(String path)
        {
            string text = System.IO.File.ReadAllText(path);
            return text;
        }

        private String browseFile()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            // dlg.Filter = "Text files (*.txt)|*.rtf|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            dlg.Filter = "Text files (*.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                return filename;
            }
            return null;

        }

        /*private void updateRtbTextFromTextFragmentContainer()
        {
            List<TextFragment> fragments = new List<TextFragment>(textFragmentcontainer.getFragments());
            foreach (TextFragment fragment in fragments)
            {
                Color colorForPerceivityStatus = fragment.getPerceivity() ? Colors.Green : Colors.Red;
               TextUtils.colorizeText(rtbText, fragment.getStartIndex(), fragment.getLength(), colorForPerceivityStatus);
            }
        }*/
        private void updateRtbTextFromTextFragment(TextFragment fragment)
        {
                Color colorForPerceivityStatus = fragment.getPerceivity() ? Colors.Green : Colors.Red;
                TextUtils.colorizeText(rtbText, fragment.getStartIndex(), fragment.getLength(), colorForPerceivityStatus);
        }

        private void finishSession()
        {
            Console.WriteLine("Perceived text:" + textFragmentcontainer.getPerceivedText());
            resultsWindow = new ResultsWindow(textFragmentcontainer);
            resultsWindow.ShowDialog();
        }

        private void btnFinish_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            finishSession();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void markFragmentReadedToNextPunctuationMark()
        {
            //TODO
            if (currentPunctuationMarkIndex >= indexesOfPunctuationMarks.Count - 1)
            {
                finishSession();
            }
            //int indexToBeMarked = indexesOfPunctuationMarks[currentPunctuationMarkIndex];//textFragmentcontainer.GetFragmentsLength()
            //end of text if no punctuation marks left
            int indexToBeMarked = (currentPunctuationMarkIndex == indexesOfPunctuationMarks.Count - 1) ? textFragmentcontainer.GetFragmentsLength() : indexesOfPunctuationMarks[currentPunctuationMarkIndex];
            createTextFragmentAndColorizeIt(indexToBeMarked);
            if (indexToBeMarked >= textFragmentcontainer.GetFragmentsLength())
            {
                finishSession();
            }
            currentPunctuationMarkIndex++;
            //textFragmentcontainer.st
        }

        private void rtbText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                markFragmentReadedToNextPunctuationMark();
            }
        }
    }
}
