using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
namespace PartsMysql.Models
{

    [Table("ncr")]
    public class Ncr
    {
        public int id { get; set; }
        public int isCompleted { get; set; }
        public string ncrNumber { get; set; }
        public string remarks { get; set; }
        public int VERSION { get; set; }


    }
}