using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class InspectionStatistics : InspectionSummary
    {
    public DateTime DateDelivered { get; set; }
    public DateTime? InspectionStart { get; set; }
    public DateTime? InspectionEnd { get; set; }
    public long? InspectionDuration { get; set; }
    public DateTime? EvaluationStart { get; set; }
    public DateTime? EvaluationEnd { get; set; }
    public long? EvaluationDuration { get; set; }
       

    }
}