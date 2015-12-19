using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using System.IO;
using System.Web.Helpers;
using Portal.Entities;


namespace Portal.Controllers
{

    [Employee]
    public class HomeController : ApplicationController
    {
        public static int docCounter = 0;
        public ActionResult Index()
        {
            string[] exts = MvcApplication.Config("gellery.Extensions", ".jpg,.gif,.swf").Split(',');
            var item = GetSession.Get<Setting>((long)1);
            var items = Directory.GetFiles(Server.MapPath("~/Public/UserFiles/gallery"))
                .Where(x => exts.Contains(Path.GetExtension(x).ToLower()))
                .Select(x => Path.GetFileName(x))
                .ToList();

            ViewBag.GalleryImages = items;
            string links = "<table class='table table-hover table-bordered'><tr><td>שם הנוהל</td><td>הורד לקריאה</td><td>אישור קריאה</td></tr>";

            IList<Doc> docs_list = GetSession.QueryOver<Doc>().Where(x => x.Active).List();


            List<Doc> availableToUserDocs = new List<Doc>();
            //var docum = null;

            if (docs_list.Count() > 0)
            {

                availableToUserDocs = (from r in docs_list
                                       where r.isAvailable2All == true || r.UsersAllowed.Contains(Employee.Current.Id.ToString())
                                       select r).ToList();

            }

            //long currentEmpDepId = GetSession.Get<Employee>(GetEmployeeId).Department.Id;
            //IList<Doc> docs_list = GetSession.QueryOver<Doc>().Where(x => x.Active).WhereRestrictionOn(x => x.Units).IsLike("%" + currentEmpDepId + "%").List();

            if (availableToUserDocs != null)
            {
                string sLink = "";
                foreach (Doc doc in availableToUserDocs)
                {
                    sLink = GetSubmitedDocs(Employee.Current.Id, doc);
                    if (sLink != "")
                    {
                        links += "<tr>";
                        links += sLink;
                        links += "</tr>";
                        sLink = "";
                    }
                }

                links += "</table>";
                ViewBag.DocCounter = docCounter;
                //ViewBag.DocCounter = docs_list.Count;
                ViewBag.Links = links;
                docCounter = 0;
            }
            else
            {
                ViewBag.DocCounter = 0;
            }

            // Departments
            item.Departments = GetSession.QueryOver<Department>().List();


            return View(item);
        }


        public ActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }


        public ActionResult Chart()
        {

            return View();
        }

        public ActionResult Reload(string returnUrl)
        {
            Session.Clear();

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index");

            return Redirect(returnUrl);
        }

        public static string GetSubmitedDocs(long userID, Doc doc)
        {
            if (string.IsNullOrEmpty(doc.Submited))
            {

                docCounter++;


                string[] arr = doc.Url.Split('.').ToArray();

                if (arr.Length > 1)
                {
                    return "<td>" + doc.Name + " </td><td><a name='submited' title='" + userID + "' alt='" + doc.Id +
                    "'  class='new_doc doc_action' target='_blank' href='/Public/userfiles/docs/" + doc.Url + "'>הורד לקריאה</a></td><td>" +
                    "<input type='checkbox' id='" + userID + "' name='" + doc.Id + "' class='checked' >" +
                    "</td>";
                }
                else
                {
                    return "<td>" + doc.Name + " </td><td><a name='submited' title='" + userID + "' alt='" + doc.Id +
            "'  class='new_doc doc_action' target='_blank' href='" + doc.Url + "'>לחץ כאן</a></td><td>" +
            "<input type='checkbox' id='" + userID + "' name='" + doc.Id + "' class='checked' >" +
            "</td>";

                }




            }
            if (doc.Url != null)
            {
                if (!doc.Submited.Split(',').Select(v => long.Parse(v)).Contains(userID))
                {

                    docCounter++;

                    string[] arr = doc.Url.Split('.').ToArray();

                    if (arr.Length > 1)
                    {
                        return "<td>" + doc.Name + " </td><td><a name='submited' title='" + userID + "' alt='" + doc.Id +
                  "'  class='new_doc doc_action' target='_blank' href='/Public/userfiles/docs/" + doc.Url + "'>הורד לקריאה</a></td><td>" +
                  "<input type='checkbox' id='" + userID + "' name='" + doc.Id + "' class='checked' >" +
                  "</td>";

                    }
                    else
                    {
                        return "<td>" + doc.Name + " </td><td><a name='submited' title='" + userID + "' alt='" + doc.Id +
                     "'  class='new_doc doc_action' target='_blank' href='" + doc.Url + "'>לחץ כאן</a></td><td>" +
                     "<input type='checkbox' id='" + userID + "' name='" + doc.Id + "' class='checked' >" +
                     "</td>";


                    }

                   
                }
                return "";
            }
            else
            {
                return "";
            }
        }
    }
}
