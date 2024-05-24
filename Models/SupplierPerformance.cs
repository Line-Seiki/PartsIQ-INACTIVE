using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace PartsMysql.Models
{
    public class SupplierPerformance
    {
        [Key] public int  Id { get; set; }
        public long SupplierId { get; set; }
        public string Decision {  get; set; }
        public string PartName { get; set; }
        public string NcrCount { get; set; }
        public string NcrNumber { get; set; }
        public string PartsCode { get; set; }
        public string SupplierName { get; set; }
        public string LotNumber { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public DateTime? DateFinished { get; set; }


    }
}