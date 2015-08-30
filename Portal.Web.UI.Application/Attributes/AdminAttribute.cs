using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using Portal.Models.Employees;

namespace Portal.Attributes
{
    public class AdminAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            var employee = httpContext.Session["employee"] as Employee;
            /*
            var admins = MvcApplication.Config("admin.Username");

            if (string.IsNullOrEmpty(admins))
                return false;

            return admins.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(employee.Username);
            */
            if (employee.IsAdmin)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}