using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.Forums;
using AutoMapper;
using MvcContrib.Pagination;
using NHibernate.Criterion;
using System.Net.Mail;
using System.Text.RegularExpressions;



namespace Portal.Controllers
{


    [Employee]
    public class ForumsController : ApplicationController
    {

        private void sendMailTo(Employee emp,int forum_id)
        {

            #region mail object
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("tagmediagroup@gmail.com", "tagmedia1234");
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            MailAddress
                maFrom = new MailAddress("alex@tmgroup.co.il", "Sender's Name"),
                maTo = new MailAddress(emp.Email.ToString(), "Recipient's Name");
            MailMessage mmsg = new MailMessage(maFrom.Address, maTo.Address);
            mmsg.Body =
                "<html><body>"
                + "<h1>שלום, " + emp.FirstName.ToString() + " !</h1>"
                + "<h2>הודעה חדשה בפורום</h2>"
                + " <a href='" + Request.Url.Host+ "/Forums/" + forum_id + "'>לחץ כאן על הקישור לכניסה לפורום</a>"
                + "</body></html>";
            mmsg.IsBodyHtml = true;
            mmsg.Subject = "הודעה חדשה בפורום";
            client.Send(mmsg);
            #endregion

        }




        public ActionResult Index(int categoryId = 1, int page = 1)
        {

            string currUser = GetSession.Get<Employee>(GetEmployeeId).Id.ToString();

            string[] names = new string[] { "one", "two", "three" };



            var currForumUsers = GetSession.Get<ForumItem>(Convert.ToInt64(categoryId)).ForumUsers;
            names = currForumUsers.ToString().Split(',');


            bool currForumAvalible = GetSession.Get<ForumItem>(Convert.ToInt64(categoryId)).isAvailableTooAll;

            if (currForumAvalible)
            {

            }
            else
            {
                if (GetSession.Get<Employee>(GetEmployeeId).IsAdmin == false)
                {
                    int index2 = Array.IndexOf(names, currUser);
                    if (index2 < 0)
                    {
                        return RedirectToAction("ErrorForum", "Employees", new { id = currUser });
                    }

                }
            }



            bool isActive = GetSession.QueryOver<ForumItem>()
                    .Where(x => x.Id == categoryId).Take(1).SingleOrDefault().Active;
            if (isActive)
            {
                var items = GetSession.QueryOver<Forum>()
                    .Where(x => x.Parent == null && x.CategoryId == categoryId)
                    .OrderBy(x => x.CreatedDate).Desc
                    .List();
                var categoryTitle = "";
                try
                {
                    categoryTitle = GetSession.QueryOver<ForumItem>()
                        .Where(x => x.Id == categoryId).Take(1).SingleOrDefault().Title;
                    categoryTitle = (string.IsNullOrEmpty(categoryTitle)) ? "" : categoryTitle;
                }
                catch (Exception)
                {
                    categoryTitle = "פורום חדש";

                }




                ViewBag.list = GetSession.QueryOver<ForumItem>().List();

                ViewData["catTitle"] = categoryTitle;

                ViewData["catId"] = categoryId;
                return View(items.AsPagination(page, 30));
            }
            else
            {
                var categoryTitle = GetSession.QueryOver<ForumItem>()
                   .Where(x => x.Id == categoryId).Take(1).SingleOrDefault().Title;
                ViewData["title"] = categoryTitle;
                ViewData["active"] = "not active";
                return View("not_active");
            }
            

        }

        public ActionResult ForumsBox(int categoryId = 1)
        {
            var items = GetSession.QueryOver<Forum>()
                .Where(x => x.Parent == null && x.CategoryId == categoryId)
                .OrderBy(x => x.CreatedDate).Desc
                .Take(5)
                .List();
            ViewData["catId"] = categoryId;
            return View(items);
        }

        public ActionResult Create(long? parentId, int categoryId = 1)
        {
            var model = new ForumFormModel();

            if (parentId.HasValue)
            {
                var parent = GetSession.Get<Forum>(parentId);
                model.ParentId = parentId;
                model.ParentSubject = parent.Subject;
                model.Subject = model.ParentSubject;
            }
            ViewData["catId"] = categoryId;

            ViewBag.list = GetSession.QueryOver<ForumItem>().List();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ForumFormModel model, int categoryId = 1)
        {
            if (ModelState.IsValid)
            {
                #region Replace url text with link
                string body = model.Body;

                var outputString = model.Body;

                Regex regx = new Regex(@"https?://([-\w\.]+)+(:\d+)?(/([\w/_\.]*(\?\S+)?)?)?", RegexOptions.IgnoreCase);

                MatchCollection mactches = regx.Matches(model.Body);
                foreach (Match match in mactches)
                {
                    outputString = outputString.Replace(match.Value, String.Format("<a href=\"{0}\" target=\"_blank\">לחץ כאן</a>", match.Value));
                }

                body = outputString;


                model.Body = body; 
                #endregion


                var item = Mapper.Map<ForumFormModel, Forum>(model);
                item.CreatedBy = new Employee
                {
                    Id = GetEmployeeId
                };

                if (model.ParentId.HasValue)
                {
                    var forum = GetSession.Load<Forum>(model.ParentId);
                    item.Parent = forum;
                    item.Subject = item.Parent.Subject; // Must be Uniqe!!!!


                }
                ViewData["catId"] = categoryId;
                item.CategoryId = categoryId;
                ViewBag.list = GetSession.QueryOver<ForumItem>().List();

                GetSession.Save(item);

                var current_domain = MvcApplication.Config("current_domain").ToString();
                string host = HttpContext.ApplicationInstance.Request.Url.Host;


                var forumItem = GetSession.QueryOver<ForumItem>().Where(x => x.Id == categoryId).SingleOrDefault().Admin;




                #region Send mail to forum users


                if (GetSession.Get<ForumItem>(Convert.ToInt64(categoryId)).isAvailableTooAll == false)
                {
                    Employee emp;
                    var forum_users = GetSession.Get<ForumItem>(Convert.ToInt64(categoryId)).ForumUsers;
                    string[] ids = forum_users.Split(',');

                    var list = new List<string>(ids);
                    list.Remove(GetEmployeeId.ToString());
                    ids = list.ToArray();

                    for (int i = 0; i < ids.Length; i++)
                    {
                        long id = Convert.ToInt64(ids[i]);
                        emp = GetSession.Get<Employee>(id);
                        sendMailTo(emp, categoryId);
                    } 
                    
                }

          


                #endregion




                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("tagmediagroup@gmail.com");
                mail.To.Add(forumItem.Email.ToString());
                mail.Subject = "הודעה חדשה בפורום";
                mail.IsBodyHtml = true;
                mail.Body = "<html><body>"
                    + "<h1>שלום, " + forumItem.FirstName.ToString() + " !</h1>"
                    + "<h2>הודעה חדשה בפורום</h2>"
                    + " <a href='" + host + "/Forums/" + item.CategoryId.ToString() + "'>לחץ כאן על הקישור לכניסה לפורום</a>"
                    + "</body></html>";

                SmtpClient smtp = new SmtpClient();
                //smtp.Send(mail);





              
                //SmtpClient client = new SmtpClient();
                //client.Credentials = new System.Net.NetworkCredential("tagmediagroup@gmail.com", "tagmedia1234");
                //client.Port = 587;
                //client.Host = "smtp.gmail.com";
                //client.EnableSsl = true;
                //MailAddress
                //    maFrom = new MailAddress("tagmediagroup@gmail.com", "Sender's Name"),
                //    maTo = new MailAddress(forumItem.Email.ToString(), "Recipient's Name");
                //MailMessage mmsg = new MailMessage(maFrom.Address, maTo.Address);
                //mmsg.Body =
                //    "<html><body>"
                //    + "<h1>שלום, " + forumItem.FirstName.ToString() + " !</h1>"
                //    + "<h2>הודעה חדשה בפורום</h2>"
                //    + " <a href='" + host + "/Forums/" + item.CategoryId.ToString() + "'>לחץ כאן על הקישור לכניסה לפורום</a>"
                //    + "</body></html>";
                //mmsg.IsBodyHtml = true;
                //mmsg.Subject = "הודעה חדשה בפורום"; 
               







                long ebatnya = 0;
                if (item.Parent != null)
                {

                    ebatnya = GetSession.QueryOver<ForumItem>().Where(x => x.Id == item.Parent.CategoryId).SingleOrDefault().Admin.Id;
                }
                else
                {
                    ebatnya = GetSession.QueryOver<ForumItem>().Where(x => x.Id == item.CategoryId).SingleOrDefault().Admin.Id;
                }




                if (item.CreatedBy.Id != ebatnya)
                {
                    smtp.Send(mail);
                }

                if (item.Parent != null)
                {
                    var theIdOfFather = GetSession.QueryOver<Forum>().Where(x => x.Parent == null).And(x => x.Subject == item.Subject).SingleOrDefault();




                    var theParentIsAdmin = GetSession.QueryOver<ForumItem>().Where(x => x.Id == item.Parent.CategoryId).SingleOrDefault();

                    if (item.CreatedBy.Id == theParentIsAdmin.Admin.Id)
                    {

                        var msgBody =
                        
                        "<html><body>"
                   + "<h1>שלום, " + theIdOfFather.CreatedBy.FirstName.ToString() + " !</h1>"
                   + "<h2>הודעה חדשה בפורום</h2>"
                   + "<a href='" + host + "/Forums/" + item.CategoryId.ToString() + "'>לחץ כאן על הקישור לכניסה לפורום</a>"
                   + "</body></html>";



                        var msgSubject = "הודעה חדשה בפורום";

                        SendForumEmail(theIdOfFather.CreatedBy.Email.ToString(), msgSubject, msgBody);

                    }

                }


                return RedirectToAction("Index");
            }
            ViewData["catId"] = categoryId;



            return View(model);
        }

        [Transaction]
        public ActionResult AutoCreate()
        {
            int categoryId = 1;
          

            var cats = GetSession.QueryOver<Forum>()
            .OrderBy(x => x.CategoryId).Desc().List();

            List<int> categories = new List<int>();

            foreach (Forum forum in cats)
            {
                if (!categories.Contains(forum.CategoryId))
                    categories.Add(forum.CategoryId);
            }
            if (categories.Count() > 0)
                categoryId = categories[0]+1;
              

            ForumFormModel model = new ForumFormModel()
            {
                Body = "פורום חדש",
                CategoryTitle = "פורום חדש",
                Subject = "פורום חדש",
                CategoryId = categoryId
            };

            var item = Mapper.Map<ForumFormModel, Forum>(model);
            item.CreatedBy = new Employee
            {
                Id = GetEmployeeId
            };
            ViewData["catId"] = categoryId;
            GetSession.Save(item);

            return RedirectToAction("List", new { categoryId = categoryId });
        }

        [HttpPost, Transaction]
        public ContentResult Destory(long id)
        {
            var item = GetSession.Get<Forum>(id);
            var forum = GetSession.Get<ForumItem>(Convert.ToInt64(item.CategoryId));

            string currentUser_email = Request.Cookies["user"].Value.ToString();


            if (forum.Admin.Email == currentUser_email)
            {

                GetSession.Delete(item);
                string js = @"alert('ההודעה נמחקה בהצלחה!')";
                return Content("111");

            }
            else
            {


                string js = @"alert('אין הרשאות לבציע את הפעולה,פנה למנהל הפורום')";
                return Content("2222");
            }

        }

        //[HttpPost, Transaction]
        //public ContentResult Destory(long id)
        //{
        //    var item = GetSession.Get<Forum>(id);
        //    var forum = GetSession.Get<ForumItem>(Convert.ToInt64(item.CategoryId));

        //    string currentUser_email = Request.Cookies["user"].Value.ToString();


        //    if (forum.Admin.Email == currentUser_email)
        //    {

        //        GetSession.Delete(item);
        //        return Content("ok");

        //    }
        //    else
        //    {
        //        return Content("ok");
        //    }
      
        //}


        //[HttpPost, Transaction]
        //public ContentResult Destory(long id)
        //{
        //    var item = GetSession.Get<Forum>(id);

        //    //if (item.CreatedBy == new Employee { Id = GetEmployeeId })
        //    //{
        //        GetSession.Delete(item);
        //        return Content("ok");
        //   // }

        //   // return Content("not owner");
        //}

        [Transaction, Authorize]
        public ActionResult DestroyMessage(long id, long categoryId)
        {
            var item = GetSession.Get<Forum>(id);
            GetSession.Delete(item);
            return RedirectToAction("List", new { categoryId });
        }

        [Authorize]
        public ActionResult List(string query, int categoryId = 1, int page = 1)
        {


            var items = GetSession.QueryOver<Forum>()
              .Where(x => x.CategoryId == categoryId)
              .OrderBy(x => x.CreatedDate)
              .Desc
               .List();
            var categoryTitle = "";
            try
            {
                categoryTitle = GetSession.QueryOver<Forum>()
             .Where(x => x.CategoryId == categoryId).Take(1).SingleOrDefault().CategoryTitle;

                categoryTitle = (string.IsNullOrEmpty(categoryTitle)) ? "" : categoryTitle;
            }
            catch (Exception)
            {

                categoryTitle = "פורום חדש";
            }


            ViewData["query"] = query;
            ViewData["categoryId"] = categoryId;
            ViewData["categoryTitle"] = categoryTitle;
            var cats = GetSession.QueryOver<Forum>()
                .OrderBy(x => x.CategoryId).Asc.List();

            List<string> categories = new List<string>();

            foreach (Forum forum in cats)
            {
                if (!categories.Contains(forum.CategoryId.ToString()))
                    categories.Add(forum.CategoryId.ToString());
            }

            List<SelectListItem> list_categories = new List<SelectListItem>();

            foreach (string cat in categories)
            {
                list_categories.Add(new SelectListItem()
                {
                    Text = cat,
                    Value = cat
                });
            }

            ViewData["categories"] = list_categories;

            return View(items.AsPagination(page, 30));
        }

        [Admin, Transaction, HttpPost]
        public ContentResult ForumTitle(long categoryId, string title)
        {
            var forums = GetSession.QueryOver<Forum>()
            .Where(x => x.CategoryId == categoryId).List();

            foreach (Forum forum in forums)
            {
                forum.CategoryTitle = title;
                GetSession.Update(forum);
            }

            return Content("ok");
        }
    }
}
