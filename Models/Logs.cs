using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartsMysql.Models
{
    public class Logs
    {
        public long LogId { get; set; }
        public DateTime? DateLog { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string LogComment { get; set; }
        public string Event {  get; set; }


    }
}
