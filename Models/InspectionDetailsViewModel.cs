using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace PartsMysql.Models
{
    public class InspectionDetailsViewModel
    {
        [Key]
        public long InspectionId { get; set; }
        public string Decision { get; set; }
        public string Evaluator { get; set; }
        public string Inspector { get; set; }

        public string Comments { get; set; }
        public string ControlNumber { get; set; }
        public DateTime? InspectionEnd { get; set; }
        public DateTime? DateFinished { get; set; }
        public DateTime? InspectionStart { get; set; }
        public double? Humidity { get; set; }
        public long? InspectionTimeSpan { get; set; }
        public string InspectorComments { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsForReports { get; set; }
        public bool? IsRewrite { get; set; }
        public string LotNumber { get; set; }
        public int? LotQuantity { get; set; }
        public int? SampleSize { get; set; }
        public int? Status { get; set; }
        public double? Temperature { get; set; }
        public long? Time { get; set; }
        public int? Version { get; set; }
        public long? DecisionId { get; set; }
        public long? DeliveryId { get; set; }
        public long? UserEvaluatorId { get; set; }
        public long? UserInspectorId { get; set; }
        public string NcrId { get; set; }
        public long? UserReservedEvaluatorId { get; set; }
        public long? PartId { get; set; }

        public string PartsCode { get; set; }
        public string PartName { get; set; }
        public string PartModel { get; set; }
        public string DrNumber { get; set; }
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierInCharge {  get; set; }


    }

   
}