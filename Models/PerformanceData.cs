using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class ChartData
    {
        public double? Lcl { get; set; } // Nullable to handle null values
        public double? Ucl { get; set; } // Nullable to handle null values
    }

    public class PerformanceData
    {
        public string Cavity { get; set; }
        public ChartData IChart { get; set; }
        public ChartData XBarChart { get; set; }
        public ChartData RChart { get; set; }
        public ChartData MrChart { get; set; }

    }
}