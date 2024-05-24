using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class ImageDataModel
    {
        public string PartName { get; set; }
        public string LowerLimit { get; set; }
        public string UpperLimit { get; set; }
        public string PartCode { get; set; }
        public string Supplier { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Cavity { get; set; }
        public string Checkpoint { get; set; }

        public string HeaderTitle { get; set; }

    }
}