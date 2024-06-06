using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    [Table("delivery")]
    public class Delivery
    {
        public int id { get; set; }
        public DateTime dateDelivered { get; set; }
        public DateTime deadline { get; set; }
        public string drNumber { get; set; }
        public int priorityLevel { get; set; }
        public int quantity { get; set; }
        public int urgent { get; set; }
        public int VERSION { get; set; }

    }
}