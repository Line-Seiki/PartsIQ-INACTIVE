using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class InspectionSummary
    {
        [Key]
        public long? InspectionId { get; set; }
        public string Decision { get; set; }
        public string Evaluator { get; set; }
        public string Inspector { get; set; }
        public string Comments { get; set; }
        public string ControlNumber { get; set; }
        public DateTime? DateFinished { get; set; }
        public string InspectorComments { get; set; }
        public string LotNumber { get; set; }
        public int? LotQuantity { get; set; }
        public int? SampleSize { get; set; }
        public long? Time { get; set; }
        public string NcrId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string DrNumber { get; set; }
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplerInCharge {  get; set; }
}

    }
