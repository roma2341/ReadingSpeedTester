using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;

namespace ReadingSpeedTester
{
    public class ViewportAxesRangeRestriction : IViewportRestriction
    {
        public DisplayRange XRange = null;
        public DisplayRange YRange = null;

        public Rect Apply(Rect oldVisible, Rect newVisible, Viewport2D viewport)
        {
            if (XRange != null)
            {
                newVisible.X = XRange.Start;
                newVisible.Width = XRange.End - XRange.Start;
            }

            if (YRange != null)
            {
                newVisible.Y = YRange.Start;
                newVisible.Height = YRange.End - YRange.Start;
            }

            return newVisible;
        }

        public event EventHandler Changed;
    }
}
