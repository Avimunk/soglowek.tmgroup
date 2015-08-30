using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using NHibernate.Linq;

namespace Portal.Attributes
{
    public class EmployeeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext) {

            if (!httpContext.Request.IsAuthenticated)
                return false;

            var employee = Employee.Current;

#if DEBUG
			employee = null;
#endif

            employee = null;

            if (employee == null && !String.IsNullOrWhiteSpace(Employee.GetUsername)) {
                using (var session = NHibernateHelper.OpenSession()) {
                    using (var t = session.BeginTransaction()) {
                        employee = session.QueryOver<Employee>()
                            .Where(x => x.Username == Employee.GetUsername)
                            .Take(1)
                            .SingleOrDefault();

                        if (employee == null) {
                            employee = new Employee {
                                Username = Employee.GetUsername,
                                FirstName = Employee.GetUsername,
                                LastName = "Name",
                                BDay = DateTime.Parse("1/1/1999"),
                                Id = new Random().Next(99999999),
                                IsActive = true
                            };
                            session.Save(employee);
                            t.Commit();
                        }

                        httpContext.Session["employee"] = employee;
                    }
                }
                return true;

            }
            return true;
        }

    }
}