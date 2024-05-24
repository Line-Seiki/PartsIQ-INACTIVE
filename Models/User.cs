using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    [Table("users")]
    public class User
    {
        public int id   { get; set; }
        public string email { get; set; }
        public int isActive { get; set; }
        public int isLoggedIn { get; set; }
        public DateTime lastUpdate { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int VERSION { get; set; }


    }
}