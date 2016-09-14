using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private int loadedTextLength = 0;

        public MainWindow()
        {
            InitializeComponent();
            rtbText.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(rtbText_MouseDown),true);
        }

         void rtbText_MouseDown(object sender, MouseButtonEventArgs e)
         {
             var senderType = sender.GetType();
            int carretPosition = TextUtils.toCarretPosition(rtbText);
             TextFragment textFragmentAfterAction;
            Console.WriteLine("Mouse(LB) preview down. Carret position:"+ carretPosition);
             if (carretPosition==null || textFragmentcontainer == null) return;
            textFragmentAfterAction = textFragmentcontainer.startOrFinishFragment((int)carretPosition);
             if (textFragmentAfterAction!=null && textFragmentAfterAction.isFinished()) updateRtbTextFromTextFragment(textFragmentAfterAction);
            if (carretPosition >= loadedTextLength)
            {
                finishSession();
            }

        }

        private void btnLoad_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            String fileText = "";
            String textFilePath = browseFile();
            if (textFilePath != null)
                fileText = loadFileContent(textFilePath);
            TextUtils.setRichTextBoxValue(rtbText, fileText);
            textFragmentcontainer = TextFragmentContainer.fromText(TextUtils.textFromRichTextBox(rtbText));
        }
 
        private string loadFileContent(String path)
        {
            string text = System.IO.File.ReadAllText(path);
            loadedTextLength = text.Length;
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
            resultsWindow = new ResultsWindow(textFragmentcontainer);
            resultsWindow.ShowDialog();
        }

        private void btnFinish_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            finishSession();
        }
    }
}
