using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    
    public class User
    {
        public long UserId   { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
    }
}