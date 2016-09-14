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

namespace ReadingSpeedTester
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        private TextFragmentContainer textFragmentcontainer;
        private Statistic statisticWindow;
        public ObservableDataSource<Point> statisticDataSource = null;
        public ResultsWindow(TextFragmentContainer textFragmentcontainer)
        {
            InitializeComponent();
           this.textFragmentcontainer = textFragmentcontainer;
            initStatisticWindow();
        }
        private void btnAnalyzeCharactersPerSecond_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
                if (fragment.getPerceivity())
                statisticDataSource.Collection.Add(new Point(totalTime / 1000, length));

            }
            statisticDataSource.ResumeUpdate();
            statisticWindow.ChartPlotterStatistic.FitToView();
            statisticWindow.Show();
            // statisticWindow.limitAxis(0, 0, 100, 100);
        }
        private void initStatisticWindow()
        {
            statisticWindow = new Statistic();
            statisticDataSource = new ObservableDataSource<Point>();
            statisticDataSource.SetXYMapping(p => p);
            statisticWindow.LinegraphStatistic.Plotter.Children.Add(new LineGraph(statisticDataSource));
            //statisticWindow.Show();
            // _noiseSource.SuspendUpdate();
            //_noiseSource.Collection.Clear();
            //_noiseSource.ResumeUpdate();
        }
    }
}
