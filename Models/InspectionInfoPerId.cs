using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class InspectionInfoPerId
    {
        public long InspectionId { get; set; }
        public string Comments { get; set; }
        public string PartName { get; set; }
        public string CCode { get; set; }
        public long? CheckpointId { get; set; }

        public double LowerLimit {  get; set; }
        public double UpperLimit { get; set;}
        public int  WithInvalid { get; set; }
        public string Specification { get; set; }

        public string Tool { get; set; }

    }
}