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
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace ReadingSpeedTester
{
    /// <summary>
    /// Interaction logic for ChartStatistic.xaml
    /// </summary>
    public partial class ChartStatistic : Window
    {
        public ChartStatistic()
        {
            InitializeComponent();
        }

        public void limitAxis(double xMin, double xMax,double yMin, double yMax)
        {
            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            restr.XRange = new DisplayRange(xMin, xMax);
            restr.YRange = new DisplayRange(yMin, yMax);
            ChartPlotterStatistic.Viewport.Restrictions.Add(restr);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
