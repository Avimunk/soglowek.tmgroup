using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using System.Text;

namespace Portal.Helpers
{
    public static class PagesHelper
    {
        public static MvcHtmlString PageName(this HtmlHelper helper, Page page)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            switch (page.TypeId)
            {
                case 1:
                    return MvcHtmlString.Create(
                        string.Format("<a href='{0}'>{1}</a>",
                            url.Action("Index", "Pages", new
                            {
                                page.SectionId,
                                parentId = page.Id
                            }),
                            page.Name)
                    );
            }

            return MvcHtmlString.Create(page.Name);
        }


        public static MvcHtmlString PageLink(this HtmlHelper helper, Page page, string type = "nav")
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);



            if (type == "main-sub")
            {
                switch (page.TypeId)
                {
                    case 1:
                    case 2:
                        return MvcHtmlString.Create(
                                string.Format("<a id='{3}-menu-{2}' href='{0}'>{1}</a>",
                                    url.Action("Show", "Pages", new
                                    {
                                        page.Id
                                    }),
                                    page.Name,
                                    page.Id,
                                    type)
                            );
                    default:
                        return MvcHtmlString.Create(
                                string.Format("<a {4} id='{3}-menu-{2}' href='{0}'>{1}</a>",
                                    page.IsIFrame ? url.Action("Show", "Pages", new { page.Id }) : page.Url,
                                    page.Name,
                                    page.Id,
                                    type,
                                    page.IsBlank ? " target='_blank'" : "")
                            );
                }
            }


            switch (page.TypeId)
            {
                case 1:
                case 2:
                    return MvcHtmlString.Create(
                        string.Format("<a id='{3}-menu-{2}' href='{0}'>{1}</a>",
                            url.Action("Show", "Pages", new
                            {
                                page.Id
                            }),
                            page.Name,
                            page.Id,
                            type)
                    );

                default:
                    return MvcHtmlString.Create(
                        string.Format("<a{2} id='right-menu-{3}' href='{0}'>{1}</a>",
                        page.IsIFrame ? url.Action("Show", "Pages", new { page.Id }) : page.Url,
                            page.Name,
                            page.IsBlank ? " target='_blank'" : "",
                            page.Id
                        )
                    );
            }
        }

        public static MvcHtmlString BreadCrumbs(this HtmlHelper helper, Page page)
        {
            var theParent = page.Parent;
            
            if (page.Parent == null)
                return MvcHtmlString.Create(page.Name);

            if (page.Parent.IsActive == false)
            {
                if (page.Parent.Parent != null)
                {
                    theParent = page.Parent.Parent;
                }
                else
                {
                    return MvcHtmlString.Create(page.Name);
                }

            }
            return MvcHtmlString.Create(string.Format("<table cellspacing='0' cellpadding='0'><tr><td> <a href='/pages/show/{2}'>{0}</a></td><td> > </td><td>{1}</td></tr></table>",
                    helper.BreadCrumbs(theParent),
                    page.Name,
                    page.Parent.Id
                ));
   


           
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, IEnumerable<SelectListItem> items, string name, long[] ids)
        {
            string tags = "<ol>";

            foreach (var item in items)
            {
                tags += string.Format("<li><input{3} class='ch' value='{0}' type='checkbox' name='{1}' />{2}</li>",
                    item.Value,
                    name,
                    item.Text,
                    ids != null && ids.Contains(long.Parse(item.Value)) ? " checked='checked'" : ""
                );
            }

            tags += "</ol>";

            return MvcHtmlString.Create(tags);
        }

        public static IList<Page> ByPermissions(this IList<Page> pages, HttpContextBase context)
        {
            var employee = context.Session["employee"] as Employee;

            return pages.Where(x =>
                (x.IsActive)
                &&
                (
                    (string.IsNullOrEmpty(x.Employee) && string.IsNullOrEmpty(x.JobTitle) && string.IsNullOrEmpty(x.Department))
                    ||
                    (
                        (!string.IsNullOrEmpty(x.Employee) && x.Employee.Split(',').Select(v => long.Parse(v)).Contains(employee.Id))
                        ||
                        (!string.IsNullOrEmpty(x.JobTitle) && (employee.JbTitle != null && x.JobTitle.Split(',').Select(v => long.Parse(v)).Contains(employee.JbTitle.Id)))
                        ||
                        (!string.IsNullOrEmpty(x.Department) && (employee.Department != null && x.Department.Split(',').Select(v => long.Parse(v)).Contains(employee.Department.Id)))
                         )
                )
             ).ToList();
        }



































    }
}