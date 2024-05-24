using PartsMysql.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace PartsMySql.Models
{
    [Table("inspection")]
    [MetadataType(typeof(InspectionMetaData))]
    public class Inspection
    {
        public int? id { get; set; } // Primary key
        public string comments { get; set; }
        public string ctrlNumber { get; set; }
        public DateTime? dateEnd { get; set; } // Nullable DateTime
        public DateTime? dateStart { get; set; } // Nullable DateTime
        public double? humidity { get; set; } // Nullable double
        public int? inspectionTimeSpan { get; set; } // Nullable int
        public string inspectorComments { get; set; }
        public int? isArchived { get; set; } // Nullable int

        public int? isForReports { get; set; } // Nullable int
        public int? isRewrite { get; set; } // Nullable int
        public string lotNumber { get; set; }
        public int? quantity { get; set; } // Nullable int
        public int? sampleSize { get; set; } // Nullable int
        public double? temperature { get; set; } // Nullable double
        public int? totalTimeSpan { get; set; } // Nullable int
        public int? VERSION { get; set; } // Nullable int

        //Foreign Keys
        public int? Decision_ID { get; set; } // Foreign key to Decision model
        public int? Delivery_ID { get; set; } // Foreign key to Delivery model
        public int? UserEvaluator_ID { get; set; } // Foreign key to User model for evaluator
        public int? UserInspector_ID { get; set; } // Foreign key to User model for inspector
        public int? NCR_ID { get; set; } // Foreign key to NCR model
        public int? UserReservedEvaluator_ID { get; set; } // Foreign key to User model for reserved evaluator

        [ForeignKey("Decision_ID")]
        public virtual Decision Decision { get; set; }

        [ForeignKey("Delivery_ID")]
        public virtual Delivery Delivery { get; set; }

        [ForeignKey("UserEvaluator_ID")]
        public virtual User UserEvaluator { get; set; }

        [ForeignKey("UserInspector_ID")]
        public virtual User UserInspector { get; set;}

        [ForeignKey("UserReservedEvaluator_ID")]
        public virtual User UserReservedEvaluator { get;set; }

        [ForeignKey("NCR_ID")]
        public virtual Ncr Ncr { get; set; }    

    }

    public class InspectionMetaData
    {
        [DisplayName("Comments")]
        public string comments { get; set; }
    }
}
