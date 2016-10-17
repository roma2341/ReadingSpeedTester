using Microsoft.Research.DynamicDataDisplay.DataSources;
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
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay;
using System.Collections;
using Microsoft.Win32;

namespace ReadingSpeedTester
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        private TextFragmentContainer textFragmentcontainer;
        private ChartStatistic chartStatisticWindow;
        private ReadingStatisticWindow readingStatisticWindow;
        public ObservableDataSource<Point> statisticDataSource = null;
        public ObservableDataSource<Point> extraStatisticDataSource = null;
        public ResultsWindow(TextFragmentContainer textFragmentcontainer)
        {
            InitializeComponent();
           this.textFragmentcontainer = textFragmentcontainer;
            initChartStatisticWindow();
            initReadingStatisticWindow();
            TextFragmentContainerStatistic statistic = textFragmentcontainer.getStatistic();
            
          charactersPerMinuteActiveReadingSpeedLabel.Content =  statistic.AverageActiveReadingSpeedCharactersPerMinute;
          charactersPerMinuteReadingSpeedLabel.Content =  statistic.AverageReadingSpeedCharactersPerMinute;
           charactersIgnoredLabel.Content = statistic.IgnoredCharacters;
           charactersReadedLabel.Content = statistic.ReadedCharacters;
            wordsReadedLabel.Content = statistic.ReadedWords;
            wordsIgnoredLabel.Content = statistic.IgnoredWords;
           readingTimeLabel.Content = TimeUtils.formatTimeToHumanReadableForm(statistic.ReadingTime);
           idleTimeLabel.Content = TimeUtils.formatTimeToHumanReadableForm(statistic.IdleTime);
            wordsPerMinuteActiveReadingSpeedLabel.Content = statistic.AverageActiveReadingSpeedWordsPerMinute;
            wordsPerMinuteReadingSpeedLabel.Content = statistic.AverageReadingSpeedWordsPerMinute;

        }

        private void showReadingStatisticWindow()
        {
            readingStatisticWindow.Show();
        }

        private void addZeroPointtoCharacter()
        {
            statisticDataSource.Collection.Add(new Point(0, 0));
        }
    
        private void initChartStatisticWindow()
        {
            chartStatisticWindow = new ChartStatistic();
            extraStatisticDataSource = new ObservableDataSource<Point>();
            statisticDataSource = new ObservableDataSource<Point>();
            statisticDataSource.SetXYMapping(p => p);
            chartStatisticWindow.ChartPlotterStatistic.Children.Add(new LineGraph(statisticDataSource));
            chartStatisticWindow.ChartPlotterStatistic.Children.Add(new LineGraph(extraStatisticDataSource));
            //chartStatisticWindow.Show();
            // _noiseSource.SuspendUpdate();
            //_noiseSource.Collection.Clear();
            //_noiseSource.ResumeUpdate();
        }

        private void initReadingStatisticWindow()
        {
            readingStatisticWindow = new ReadingStatisticWindow();
        }

        

        /*private void btnAnalyzeCharactersPerSecondActive_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            int totalLength = 0;
           preChartDisplaying();
            foreach (TextFragment fragment in fragments)
            {

                int length = fragment.getLength();
                long time = fragment.getActivity().getTotalActiveTimeMs();
                totalTime += time;
                totalLength = length;
                 if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, totalLength));


            }
            postChartDisplaying();
            chartStatisticWindow.Show();
        }*/

        /*private void btnAnalyzeCharactersPerSecondWithoutAccumulating_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            preChartDisplaying();
            int totalLength = 0;
            foreach (TextFragment fragment in fragments)
            {

                int length = fragment.getLength();
                totalLength += length;
                long time = fragment.getActivity().getTotalTimeMs();
                totalTime += time;
                 if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, length));
                 else
                    statisticDataSource.Collection.Add(new Point(totalTime / 1000, 0));


            }
            int totalTimeSec = (int)(totalTime/1000);
            int averageLength = totalTimeSec == 0 ? 0 : totalLength / totalTimeSec;
            extraStatisticDataSource.Collection.Add(new Point(0, averageLength));
            extraStatisticDataSource.Collection.Add(new Point(totalTimeSec, averageLength));
            postChartDisplaying();
            chartStatisticWindow.Show();
        }*/
        private void preChartDisplaying()
        {
            statisticDataSource.SuspendUpdate();
            extraStatisticDataSource.SuspendUpdate();
            statisticDataSource.Collection.Clear();
            extraStatisticDataSource.Collection.Clear();
            addZeroPointtoCharacter();
        }

        private void postChartDisplaying()
        {
            statisticDataSource.ResumeUpdate();
            extraStatisticDataSource.ResumeUpdate();
            chartStatisticWindow.ChartPlotterStatistic.FitToView();
        }

       /* private void btnAnalyzeCharactersPerSecondRegardingZero_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            int charsCount = 0;
            preChartDisplaying();
            foreach (TextFragment fragment in fragments)
            {

                int length = fragment.getLength();
                long time = fragment.getActivity().getTotalTimeMs();
                totalTime += time;
                if (!fragment.getPerceivity()) length = -length;
                charsCount += length;
                // if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, length));           
            }
            int totalTimeMs = (int) (totalTime / 1000);
            int averageLength = (int)(charsCount/ totalTimeMs);
            extraStatisticDataSource.Collection.Add(new Point(0, averageLength));
            extraStatisticDataSource.Collection.Add(new Point(totalTimeMs, averageLength));
            postChartDisplaying();
            chartStatisticWindow.Show();
        }*/

        private void btnShowReadingStatistic_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           showReadingStatisticWindow();
            displayReadingStatistic();

        }

        private void addColumnToDataGrid(DataGrid dataGrid,String textOfLabel,String binding)
        {
        DataGridTextColumn textColumn = new DataGridTextColumn();
        textColumn.Header = textOfLabel; 
        textColumn.Binding = new Binding(binding);
        dataGrid.Columns.Add(textColumn);
        }
        private void initReadingStatisticDataGridForTuple(String header1,String header2)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            //init labels
            readingStatisticWindow.dataGrid.Columns.Clear();
            addColumnToDataGrid(readingStatisticWindow.dataGrid,header1,"Item1");
            addColumnToDataGrid(readingStatisticWindow.dataGrid, header2,"Item2");


        }
    private void displayReadingStatistic()
    {
            initReadingStatisticDataGridForTuple("Тип","Значення");
        var statistic = textFragmentcontainer.getStatisticPerFragment();
        foreach (var statisticItem in statistic)
        {
            readingStatisticWindow.dataGrid.Items.Add(statisticItem);
        }


    }
        //WITH ACCUMULATING
        private void displayChartData(List<Point> points, List<Point> extraPoints=null)
        {
            preChartDisplaying();
            foreach (Point pt in points)
            {
            statisticDataSource.Collection.Add(pt);
            }
            if (extraPoints != null)
            {
                foreach (Point extraPt in extraPoints)
                {
                    extraStatisticDataSource.Collection.Add(extraPt);
                }
            }
            postChartDisplaying();
            chartStatisticWindow.Show();
            // chartStatisticWindow.limitAxis(0, 0, 100, 100);
        }

        public void displayStatisticChart(ChartStatisticCriteria criteria)
        {
            var chartStatisticPointsLists = textFragmentcontainer.generateStatisticChartData(criteria);
            Object pointsObject = null, extraPointsObject = null;
            bool pointsReceived = chartStatisticPointsLists.TryGetValue("points", out pointsObject);
            if (!pointsReceived) return;
            chartStatisticPointsLists.TryGetValue("extraPoints", out extraPointsObject);

            List<Point> points = (List<Point>)pointsObject;
            List<Point> extraPoints = extraPointsObject == null ? null : (List<Point>)extraPointsObject;
            displayChartData(points, extraPoints);
        }
        private void btnShowChart_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            bool countPause = true;
            bool wordsMeasure = false;
            ChartStatisticCriteria criteria = new ChartStatisticCriteria();
            criteria.Accumulating = (bool)cbChartAccumulating.IsChecked;
            criteria.IncludeReaded = (bool)cbChartIncludeReaded.IsChecked;
            criteria.IncludeSkipped = (bool)cbChartIncludeSkipped.IsChecked;
            criteria.WordsMeasure = (bool)cbChartWordsMeasure.IsChecked;
            criteria.IncludePauseTime = (bool)cbChartIncludePauseTime.IsChecked;
            criteria.BaseLine = (bool)cbChartBaseLine.IsChecked;
            criteria.Average = (bool)cbChartAverage.IsChecked;     
            displayStatisticChart(criteria);
        }

        private void btnSaveStatisticToFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            saveToExcel();
        }

        private void saveToExcel()
        {
            Microsoft.Office.Interop.Excel.Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Excel.Range rangeToHoldHyperlink;
            Microsoft.Office.Interop.Excel.Range CellInstance;
            xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            //  xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlApp.ActiveSheet;
            xlApp.DisplayAlerts = false;
            //Dummy initialisation to prevent errors.
            rangeToHoldHyperlink = xlWorkSheet.get_Range("A1", Type.Missing);
            CellInstance = xlWorkSheet.get_Range("A1", Type.Missing);
            var statistic = textFragmentcontainer.getStatisticPerFragment();
            foreach (var statisticItem in statistic)
            {
                readingStatisticWindow.dataGrid.Items.Add(statisticItem);
            }

            for (int i = 0; i < statistic.Count; i++)
            {
                var statisticItem = statistic[i];
                xlWorkSheet.Cells[i + 1,1] = statisticItem.Item1;
                xlWorkSheet.Cells[i + 1,2] = statisticItem.Item2;
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if ((bool)saveFileDialog1.ShowDialog())
            {
                xlWorkBook.SaveAs(saveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            }          
            xlWorkBook.Close();
        }
    }
}
