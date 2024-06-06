using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PartsMysql.Models
{
    public class LargeJsonResult : JsonResult
    {
        public LargeJsonResult() {
            MaxJsonLength = int.MaxValue;
            RecursionLimit = 100;
        }

        public int? MaxJsonLength { get; set; }
        public int? RecursionLimit { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JSON GET is not allowed");
            }

            if (Data == null)
            {
                return;
            }

            var serializer = new JavaScriptSerializer();

            if (MaxJsonLength.HasValue)
            {
                serializer.MaxJsonLength = MaxJsonLength.Value;
            }

            if (RecursionLimit.HasValue)
            {
                serializer.RecursionLimit = RecursionLimit.Value;
            }

            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            Encoding encoding = ContentEncoding ?? Encoding.UTF8;
            response.ContentEncoding = encoding;
            response.Write(serializer.Serialize(Data));
        }
    }
}
