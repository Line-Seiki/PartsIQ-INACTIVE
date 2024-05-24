using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PartsMysql.Models
{
    public class InspectionInfo : InspectionDetailsViewModel
    {
        public long? CavityId { get; set; }
        public string CavityNumber { get; set; }
        public int? CavityCount { get; set; }

        public DateTime ? DateDelivered { get; set; }
        public string DocNumber { get; set; }




    }
}