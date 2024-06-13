
using PartsMysql.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LSMTS.Utility
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {


        private readonly PartsIQEntities _db;
        private readonly string _moduleName;
        private readonly string _subModuleName;

        public CustomAuthorizeAttribute(string moduleName, string subModuleName)
        {
            _db = new PartsIQEntities();
            _moduleName = moduleName;
            _subModuleName = subModuleName;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the user is authenticated
            if (httpContext.User.Identity.IsAuthenticated)
            {

                var session = httpContext.Session["Menu"] as string;

                if (session != null)
                {
                    // Deserialize the JSON string into a Dictionary<string, List<string>>
                    var menu = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(session);

                    // Check if both _moduleName and _subModuleName exist in the menu
                    if (menu.ContainsKey(_moduleName.ToString()) && menu[_moduleName.ToString()].Contains(_subModuleName.ToString()))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Handle the case where the session data is not available
                    return false;
                }

            }
            else
                return false;
        }

    }
}