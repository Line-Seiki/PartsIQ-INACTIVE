using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace PartsMysql.Models
{

    [Table("decision")]
    public class Decision
    {
        public int? id { get; set; }
        public int? isArchived { get; set; }
        public string name { get; set; }
        public int VERSION { get; set; }

    }
}