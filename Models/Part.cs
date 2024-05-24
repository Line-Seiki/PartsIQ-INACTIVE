using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartsMysql.Models
{
    public class Part
    {
        public long PartId { get; set; }
        public string PartCode { get; set; }
        public string DocNumber { get; set; }
        public string PartModel { get; set; }
        public string PartName { get; set; }
        public string PartType { get; set; }
        public long? PartFileAttachment  {  get; set; }

    }
}