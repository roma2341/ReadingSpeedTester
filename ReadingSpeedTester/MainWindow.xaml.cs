using System;
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
        private bool isPlainTextLoaded = false;
        private TextPointer previousCarretPosition = null;
        String fileContent = "";

        private void markPlainTextLoaded()
        {
            isPlainTextLoaded = true;
        }

        private void markCompositeTextLoaded()
        {
            isPlainTextLoaded = false;
        }

        private Boolean isCompositeTextLoaded()
        {
            return !isPlainTextLoaded;
        }
        
      

        public MainWindow()
        {
            InitializeComponent();
            rtbText.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(rtbText_MouseLeftDown),true);
            rtbText.AddHandler(FrameworkElement.MouseRightButtonDownEvent, new MouseButtonEventHandler(rtbText_MouseRightDown), true);
        }


        void createTextFragmentAndColorizeIt(int carretPosition)
        {
            int textFragmentEndPosition = carretPosition - 1;
            if (textFragmentcontainer==null || textFragmentEndPosition <= textFragmentcontainer.GetLastIndex()) return;
            TextFragment textFragmentAfterAction;
            if (textFragmentEndPosition == null || textFragmentcontainer == null) return;
            textFragmentAfterAction = textFragmentcontainer.addPerceivedFragment(textFragmentEndPosition);
            updateRtbTextFromTextFragment(textFragmentAfterAction);
            if (SessionFinishingPossible(textFragmentEndPosition))
            {
                finishSession();
            }
        }
        void moveLastFragmentIndexAndColorizeIt(int carretPosition)
        {
            int moveToPosition = carretPosition - 1;
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
            Console.WriteLine("fragments length:"+ textFragmentcontainer.GetFragmentsLength());
            return carretPosition >= textFragmentcontainer.GetFragmentsLength();
        }

        bool isDataLoaded()
        {
            return textFragmentcontainer!=null;
        }

        public void updatePreviousCarretPosition(TextPointer textPointer)
        {
            this.previousCarretPosition = textPointer;
        }
         void rtbText_MouseLeftDown(object sender, MouseButtonEventArgs e)
         {
             if (!isDataLoaded()) return;
            unPause();
            var senderType = sender.GetType();
            int carretPosition = TextUtils.toCarretPosition(rtbText);
            Console.WriteLine("left click carret position:" + carretPosition);
            createTextFragmentAndColorizeIt(carretPosition);
             updatePreviousCarretPosition(rtbText.CaretPosition);
            if (SessionFinishingPossible(carretPosition))
            {
                finishSession();
            }

        }
        void rtbText_MouseRightDown(object sender, MouseButtonEventArgs e)
        {
            if (!isDataLoaded()) return;
            unPause();
            //set text cursor position;
            RichTextBox box = (RichTextBox)sender;
            box.CaretPosition = box.GetPositionFromPoint(e.GetPosition(box), true);

            var senderType = sender.GetType();
            int carretPosition = TextUtils.toCarretPosition(rtbText);//isCompositeTextLoaded()...
            Console.WriteLine("right click carret position:"+carretPosition);
            moveLastFragmentIndexAndColorizeIt(carretPosition);
            updatePreviousCarretPosition(rtbText.CaretPosition);
            if (SessionFinishingPossible(carretPosition))
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
            string extension="";
            String textFilePath = browseFile(out extension);
            if (textFilePath != null)
            {
                if (object.Equals(extension,".txt")) {
                    try
                    {
                        fileContent = loadFileContent(textFilePath);
                        markPlainTextLoaded();
                    }
                    catch (System.IO.IOException exc)
                    {
                        MessageBoxResult result = MessageBox.Show("Файл вже використовується", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    TextUtils.setRichTextBoxValue(rtbText, fileContent);
                addPunctuationMarksFromString(fileContent);
                currentPunctuationMarkIndex = 0;
                }
                if (object.Equals(extension, ".rtf"))
                {
                    try
                    {
                        fileContent = loadFileContent(textFilePath);
                        markCompositeTextLoaded();
                       
                    }
                    catch (System.IO.IOException exc)
                    {
                        MessageBoxResult result = MessageBox.Show("Файл вже використовується", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string text =  TextUtils.setRichTextBoxRtf(rtbText, fileContent);
                    addPunctuationMarksFromString(text);
                    currentPunctuationMarkIndex = 0;
                }
            }

            textFragmentcontainer = TextFragmentContainer.fromText(TextUtils.textFromRichTextBox(rtbText));
        }
 
        private string loadFileContent(String path)
        {
            string text = System.IO.File.ReadAllText(path);
            return text;
        }

        private String browseFile(out String fileExtension)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".txt";
            // dlg.Filter = "Text files (*.txt)|*.rtf|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            dlg.Filter = "Text files (*.txt)|*.txt|(*.rtf)|*.rtf";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                string ext = System.IO.Path.GetExtension(dlg.FileName);
                fileExtension = ext;
                return filename;
            }
            fileExtension = "";
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
            string richText = new TextRange(rtbText.Document.ContentStart, rtbText.Document.ContentEnd).Text;
            Console.WriteLine("fileConentSelected:" + richText);
            TextRange range = null;
            if (previousCarretPosition!=null) range = new TextRange(previousCarretPosition, rtbText.CaretPosition);
            else range = new TextRange(rtbText.Document.ContentStart, rtbText.CaretPosition);
                    //.Substring(fragment.getStartIndex(),fragment.getLength()));
            Color colorForPerceivityStatus = fragment.getPerceivity() ? Colors.Green : Colors.Red;
            TextUtils.colorizeTextRange(rtbText,range, colorForPerceivityStatus);
        }

        private void finishSession()
        {
            string[] fragments = textFragmentcontainer.getFragmentsStrings();
            foreach (string fragmentStr in fragments)
            {
                Console.WriteLine("Fragment:"+ fragmentStr);
            }
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
            if (!isDataLoaded()) return;
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

        private bool togglePause()
        {
            bool isPaused = textFragmentcontainer.toggleReadingActivity();
            if (isPaused) rtbText.Background = Brushes.Yellow;
            else rtbText.Background = Brushes.White;
            return isPaused;
        }

        public void unPause()
        {
            if (textFragmentcontainer == null) return;
            textFragmentcontainer.resumeReadingActivity();
            rtbText.Background = Brushes.White;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                e.Handled = true;
                togglePause();
            }
        }
    }
}
