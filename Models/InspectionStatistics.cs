﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class InspectionStatistics : InspectionSummary
    {
        public long? PartId { get; set; }
        public DateTime? DateDelivered { get; set; }
        public DateTime? InspectionStart { get; set; }
        public DateTime? InspectionEnd { get; set; }
        public long? InspectionDuration { get; set; }
        public DateTime? EvaluationStart { get; set; }
        public DateTime? EvaluationEnd { get; set; }
        public long? EvaluationDuration { get; set; }


    }

    public class InspectionStatsSummary
    {
        public int TotalPartsInspected { get; set; }
        public TimeSpan MaxInspectionTime { get; set; }
        public TimeSpan MinInspectionTime { get; set; }
        public TimeSpan AverageInspectionTime { get; set; }

        public InspectionStatsSummary Calculate (List<InspectionStatistics> stats)
        {
            if (stats == null || stats.Count == 0)
            {
                return new InspectionStatsSummary
                {
                    TotalPartsInspected = 0,
                    MaxInspectionTime = TimeSpan.Zero,
                    MinInspectionTime = TimeSpan.Zero,
                    AverageInspectionTime = TimeSpan.Zero
                };
            }

            var totalInspectionTime = stats.Sum(s => s.InspectionDuration ?? 0);
            var maxInspectionTime = stats.Max(s => s.InspectionDuration ?? 0);
            var minInspectionTime = stats.Min(s => s.InspectionDuration ?? 0);

            return new InspectionStatsSummary
            {
                TotalPartsInspected = stats.Count,
                MaxInspectionTime = TimeSpan.FromMilliseconds(maxInspectionTime),
                MinInspectionTime = TimeSpan.FromMilliseconds(minInspectionTime),
                AverageInspectionTime = TimeSpan.FromMilliseconds(totalInspectionTime / stats.Count)
            };
        }

    }


}