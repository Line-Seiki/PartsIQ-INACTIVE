using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class PartDetail
    {
        public string PartName { get; set; }
        public int NcrCount { get; set; }
    }
    public class DecisionGroup : Dictionary<string, PartDetail>
    {
    }

    public class SupplierPerformanceGroup
    {
        public DecisionGroup Accept { get; set; }
        public DecisionGroup SpecialAccept { get; set; }
        public DecisionGroup SortOut { get; set; }
        public DecisionGroup Reject { get; set; }
    }
}