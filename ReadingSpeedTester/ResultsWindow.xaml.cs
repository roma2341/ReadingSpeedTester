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
        private void btnAnalyzeCharactersPerSecond_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            int totalLength = 0;
            statisticDataSource.SuspendUpdate();
            statisticDataSource.Collection.Clear();
            foreach (TextFragment fragment in fragments)
            {
              
                int length = fragment.getLength();
                long time = fragment.getTimeDeltaMS();
                totalTime += time;
                totalLength += length;
               // if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, totalLength));


            }
            statisticDataSource.ResumeUpdate();
            chartStatisticWindow.ChartPlotterStatistic.FitToView();
            chartStatisticWindow.Show();
            // chartStatisticWindow.limitAxis(0, 0, 100, 100);
        }
        private void initChartStatisticWindow()
        {
            chartStatisticWindow = new ChartStatistic();
            statisticDataSource = new ObservableDataSource<Point>();
            statisticDataSource.SetXYMapping(p => p);
            chartStatisticWindow.LinegraphStatistic.Plotter.Children.Add(new LineGraph(statisticDataSource));
            //chartStatisticWindow.Show();
            // _noiseSource.SuspendUpdate();
            //_noiseSource.Collection.Clear();
            //_noiseSource.ResumeUpdate();
        }

        private void initReadingStatisticWindow()
        {
            readingStatisticWindow = new ReadingStatisticWindow();
        }

        private void btnAnalyzeCharactersPerSecondActive_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            int totalLength = 0;
            statisticDataSource.SuspendUpdate();
            statisticDataSource.Collection.Clear();
            foreach (TextFragment fragment in fragments)
            {

                int length = fragment.getLength();
                long time = fragment.getTimeDeltaMS();
                totalTime += time;
                totalLength = length;
                 if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, totalLength));


            }
            statisticDataSource.ResumeUpdate();
            chartStatisticWindow.ChartPlotterStatistic.FitToView();
            chartStatisticWindow.Show();
        }

        private void btnAnalyzeCharactersPerSecondWithoutAccumulating_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            statisticDataSource.SuspendUpdate();
            statisticDataSource.Collection.Clear();
            foreach (TextFragment fragment in fragments)
            {

                int length = fragment.getLength();
                long time = fragment.getTimeDeltaMS();
                totalTime += time;
                // if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, length));


            }
            statisticDataSource.ResumeUpdate();
            chartStatisticWindow.ChartPlotterStatistic.FitToView();
            chartStatisticWindow.Show();
        }

        private void btnAnalyzeCharactersPerSecondRegardingZero_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            List<TextFragment> fragments = textFragmentcontainer.getFragments();
            long totalTime = 0;
            statisticDataSource.SuspendUpdate();
            statisticDataSource.Collection.Clear();
            foreach (TextFragment fragment in fragments)
            {

                int length = fragment.getLength();
                long time = fragment.getTimeDeltaMS();
                totalTime += time;
                if (!fragment.getPerceivity()) length = -length;
                // if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, length));


            }
            statisticDataSource.ResumeUpdate();
            chartStatisticWindow.ChartPlotterStatistic.FitToView();
            chartStatisticWindow.Show();
        }

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

        private void btnAnalyzeCharactersPerSecondRegardingZero_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
