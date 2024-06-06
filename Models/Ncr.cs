using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace PartsMysql.Models
{

    
    public class Ncr
    {
        public long NcrId { get; set; }
        public bool IsCompleted { get; set; }
        public string NcrNumber { get; set; }
        public string Remarks { get; set; }
        public int VERSION { get; set; }


    }

    public class NcrDetails : Ncr {
        public long? InspectionId { get; set; }

        public string Decision {  get; set; }
        public DateTime? DateInspected { get; set; }
        public string ControlNumber { get; set; }
        public string LotNumber { get; set; }
        public string InspectionComment { get; set; }
        public string DrNumber { get; set; }
        public DateTime? DateEnded { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PartModel { get; set; }
        public string SupplierName { get; set; }
        public string SupplierInCharge { get; set; }
    }
}