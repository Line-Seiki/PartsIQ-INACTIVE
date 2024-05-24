using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class Suppliers
    {
        [Key] public long Id { get; set; }

        public string InCharge { get; set; }
        public string Name { get; set; }
        public int? Version { get; set; }


    }
}