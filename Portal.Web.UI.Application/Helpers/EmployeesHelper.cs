using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using System.Web.WebPages;
using System.Web.Mvc;
using System.IO;

using Portal.Web.UI;

namespace Portal.Helpers
{
	public static class EmployeesHelper
	{
        public static Employee GetCurrentEmployee()
        {
            return (HttpContext.Current.Session["employee"] ?? new Employee()) as Employee;
        }
        public static Employee Employee(this HtmlHelper helper)
        {

           
			return (helper.ViewContext.RequestContext.HttpContext.Session["employee"] ?? new Employee()) as Employee;
		}

        public static MvcHtmlString EmployeeImage(this HtmlHelper helper, Employee item)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
           var path = HttpContext.Current.Server.MapPath("~/public/userfiles/employees/" + item.Id + ".jpg");
            if (File.Exists(path))
            {
                return MvcHtmlString.Create(
                    string.Format("<a href='{3}' class='employee-photo'><img title='{1}' src='{2}' /></a>",
                        url.Content("~/public/images/pix.gif"),
                        item.FullName,
                        url.Content("~/public/userfiles/employees/" + item.Id + ".jpg"),
                        url.Action("Card", "Employees", new { item.Id })
                    )
                );
            }
            else
            {
                return MvcHtmlString.Create(
              string.Format("<a href='{3}' class='employee-photo'><img title='{1}' style='background-size: 100%;background-image: url({2})' src='{0}' /></a>",
                  url.Content("~/public/images/pix.gif"),
                  item.FullName,
                  url.Content("~/public/images/picture_bg.jpg"),
                  url.Action("Card", "Employees", new { item.Id })
              )
          );
            }
        }

        public static MvcHtmlString EmployeeImageRight(this HtmlHelper helper, Employee item)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var path = HttpContext.Current.Server.MapPath("~/public/userfiles/employees/" + item.Id + ".jpg");
            if (File.Exists(path))
            {
                return MvcHtmlString.Create(
                    string.Format("<a href='{3}' class='employee-photo top-photo'><img title='{1}'  src='{2}' /></a>",
                        url.Content("~/public/images/pix.gif"),
                        item.FullName,
                        url.Content("~/public/userfiles/employees/" + item.Id + ".jpg"),
                        url.Action("Card", "Employees", new { item.Id })
                    )
                );
            }
            else
            {
                return MvcHtmlString.Create(
              string.Format("<a href='{3}' class='employee-photo'><img title='{1}' style='background-size: 100%;background-image: url({2})' src='{0}' /></a>",
                  url.Content("~/public/images/pix.gif"),
                  item.FullName,
                  url.Content("~/public/images/picture_bg.jpg"),
                  url.Action("Card", "Employees", new { item.Id })
              )
          );
            }
        }

        public static MvcHtmlString EmployeeImageSmall(this HtmlHelper helper, Employee item)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var path = HttpContext.Current.Server.MapPath("~/public/userfiles/employees/" + item.Id + ".jpg");
            if (File.Exists(path))
            {
                return MvcHtmlString.Create(
                    string.Format("<a href='{3}' class='employee-photo'><img title='{1}' src='{2}' /></a>",
                        url.Content("~/public/images/pix.gif"),
                        item.FullName,
                        url.Content("~/public/userfiles/employees/" + item.Id + ".jpg"),
                        url.Action("Card", "Employees", new { item.Id })
                    )
                );
            }
            else
            {
                return MvcHtmlString.Create(
              string.Format("<a href='{3}' class='employee-photo'><img title='{1}' style='background-size: 100%;background-image: url({2})' src='{0}' /></a>",
                  url.Content("~/public/images/pix.gif"),
                  item.FullName,
                  url.Content("~/public/images/picture_bg.jpg"),
                  url.Action("Card", "Employees", new { item.Id })
              )
          );
            }
        }

		public static bool IsOwner(this ViewContext context, Employee employee) {
            return employee != null && Portal.Entities.Employee.GetUsername == employee.Username;
		}

        public static TreeNode GetNode(Employee item, IEnumerable<Employee> items)
        {
            TreeNode node = new TreeNode()
            {
                ThumbImageUrl = "/public/userfiles/employees/1.jpg",
                Title = item.FullName,
                Caption = item.JobTitleName,
                Action = "Card",
                Controller = "Employees",
                Id = item.Id
            };


            IEnumerable<Employee> employees = items
                .Where(x => x.Manager != null && x.Manager.Id == item.Id).ToArray();

            foreach (Employee employee in employees)
            {
                node.AddChild(GetNode(employee, items));
            }

            return node;
        }

        public static Tree GetEmployeesTree(Department department, IEnumerable<Employee> items)
        {
            Tree tree = new Tree();

            if (department != null)
            {
                items = items.Where(x => x.Department != null && x.Department.Id == department.Id);
                tree.RootTitle = department.Name;
            }
            else
            {
                tree.RootTitle = "";
            }

            IEnumerable<Employee> rootItems = items.Where(i => i.Manager == null).Take(3).ToArray();

            foreach (Employee rootItem in rootItems)
            {
                tree.AddNode(GetNode(rootItem, items));
            }

            return tree;
        }    
	}
}