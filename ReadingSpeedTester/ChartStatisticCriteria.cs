using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReadingSpeedTester
{
   public class ChartStatisticCriteria
    {
        public bool Accumulating { get; set; }
        public bool IncludeReaded{ get; set; }
        public bool IncludeSkipped { get; set; }
        public bool WordsMeasure { get; set; }
        public bool IncludePauseTime { get; set; }
        public bool BaseLine { get; set; }
        public bool Average { get; set; }

    public ChartStatisticCriteria()
        {
            Accumulating = false;
            IncludeReaded = true;
            IncludeSkipped = false;
            WordsMeasure = false;
            IncludePauseTime = false;
            BaseLine = false;
            Average = false;
        }
    }
}
