using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using Portal.Attributes;
using System.Xml.Linq;
using NHibernate.Criterion;
using Portal.Models.Employees;
using NHibernate.Transform;
using NHibernate.Linq;
using System.Web.Helpers;
using System.IO;
using AutoMapper;
using System.Net.Mail;

namespace Portal.Controllers
{
    [Employee]
    public class EmployeesController : ApplicationController
    {
        public ActionResult Index()
        {


            ViewBag.JobTitles = GetSession.QueryOver<JobTitle>()
                .OrderBy(x => x.Name).Asc
                .List()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

            ViewBag.Departments = GetSession.QueryOver<Department>()
                .OrderBy(x => x.Name).Asc
                .List()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            return View();
        }


        [Admin]
        public ActionResult List()
        {
            var items = GetSession.QueryOver<Employee>().List();
            return View(items);
        }

        [Admin]
        public ActionResult Create()
        {
            var model = new EmployeeFormModel() { BDay = DateTime.Now };

            model.InitMembers(GetSession);

            return View(model);
        }

        [Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Create(EmployeeFormModel model)
        {
            if (ModelState.IsValid)
            {
                var item = Mapper.Map<EmployeeFormModel, Employee>(model);
                GetSession.Save(item);
                return RedirectToAction("List");
            }

            model.InitMembers(GetSession);

            return View(model);
        }

        [Admin]
        public ActionResult Edit(long id)
        {
            var item = GetSession.Get<Employee>(id);

            var model = Mapper.Map<Employee, EmployeeFormModel>(item);

            model.InitMembers(GetSession);

            return View(model);
        }

        [Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Edit(EmployeeFormModel model)
        {

            if (ModelState.IsValid)
            {
                var item = GetSession.Get<Employee>(model.Id);

                Mapper.Map<EmployeeFormModel, Employee>(model, item);

                GetSession.Update(item);

                return RedirectToAction("List");
            }

            model.InitMembers(GetSession);

            return View(model);
        }



        public ActionResult SearchUsers(string phoneq, string aliasq, int? departmentId)
        {
            if (phoneq == null)
            {
                phoneq = "";
            }


            if (aliasq == null)
            {
                aliasq = "";
            }

            if (!String.IsNullOrWhiteSpace(phoneq))
            {
                string[] words = phoneq.Split(' ');
                List<Employee> list = new List<Employee>();

                if (words.Count() > 1)
                {
                    //var items = GetSession.QueryOver<Employee>()
                    //    .Where(x => (x.LastName.IsLike(words[0], MatchMode.Anywhere) && x.FirstName.IsLike(words[1], MatchMode.Anywhere) ||
                    //        (x.LastName.IsLike(words[1], MatchMode.Anywhere) && x.FirstName.IsLike(words[0], MatchMode.Anywhere))
                    //        )).OrderBy(x => x.FirstName).Asc
                    //.List();

                    var ppl = from p in GetSession.QueryOver<Employee>().Where(p => p.IsActive).List()
                              where ((p.FirstName + " " + p.LastName).Contains(phoneq.TrimEnd().TrimStart()) ||
                              (aliasq != null && aliasq.Trim().Length > 0 && p.Alias != null && p.Alias.Trim().Length > 0 && p.Alias.Contains(aliasq.TrimEnd().TrimStart()))) &&
                              (p.IsActive)
                              select p;

                    if (departmentId.HasValue)
                    {
                        ppl = from p in ppl
                              where (p.Department != null && p.Department.Id == departmentId.Value)
                              select p;
                    }


                    ViewBag.Query = phoneq;
                    return View(ppl);

                }
                else
                {
                    var items = GetSession.QueryOver<Employee>()
                        .Where(x => x.IsActive)
                  .Where(x =>
                      (x.Phone.IsLike(phoneq, MatchMode.Anywhere) || x.LastName.IsLike(phoneq, MatchMode.Anywhere) || x.FirstName.IsLike(phoneq, MatchMode.Anywhere)) ||
                      (aliasq != null && aliasq.Trim().Length > 0 && x.Alias != null && x.Alias.IsLike(aliasq, MatchMode.Anywhere)) &&
                      (x.IsActive))
                  .OrderBy(x => x.FirstName).Asc
                  .List();

                    if (departmentId.HasValue)
                    {
                        items = items.Where(p => p.Department != null && p.Department.Id == departmentId.Value).ToList();
                    }


                    //var items = from r in GetSession.QueryOver<Employee>()
                    //where r.Phone.Contains(phoneq)
                    //select r;
                    ViewBag.Query = phoneq;

                    return View(items);
                }
            }
            else
            {
                var items = GetSession.QueryOver<Employee>()
                    .Where(x => x.IsActive)
                    .List();

                if (!String.IsNullOrWhiteSpace(aliasq))
                {
                    items = items.Where(p => !String.IsNullOrWhiteSpace(p.Alias) && p.Alias.Contains(aliasq.TrimEnd().TrimStart())).ToList();
                }


                if (departmentId.HasValue)
                {
                    items = items.Where(p => p.Department != null && p.Department.Id == departmentId.Value).ToList();
                }


                //var items = from r in GetSession.QueryOver<Employee>()
                //where r.Phone.Contains(phoneq)
                //select r;
                ViewBag.Query = phoneq;

                return View(items);
            }
        }


        public ActionResult Search(SearchFormModel model)
        {

            if (model.FirstName == "שם פרטי")
                model.FirstName = "";

            if (model.LastName == "שם משפחה")
                model.LastName = "";

            var items = GetSession.QueryOver<Employee>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.FirstName).Asc
                .OrderBy(x => x.LastName).Asc;

            var jobs = GetSession.QueryOver<JobTitle>()
                .Where(x => x.Id == model.JobTitleId).SingleOrDefault();

            if (!string.IsNullOrEmpty(model.FirstName))
                items.Where(
                    Restrictions.On<Employee>(x => x.FirstName).IsLike(model.FirstName, MatchMode.Anywhere)
                    ||
                    Restrictions.On<Employee>(x => x.LastName).IsLike(model.FirstName, MatchMode.Anywhere)
                    ||
                    Restrictions.On<Employee>(x => x.EnglishFirstName).IsLike(model.FirstName, MatchMode.Anywhere)
                );

            if (!string.IsNullOrEmpty(model.LastName))
                items.Where(
                    Restrictions.On<Employee>(x => x.LastName).IsLike(model.LastName, MatchMode.Anywhere)
                    ||
                    Restrictions.On<Employee>(x => x.FirstName).IsLike(model.LastName, MatchMode.Anywhere)
                    ||
                    Restrictions.On<Employee>(x => x.EnglishLastName).IsLike(model.LastName, MatchMode.Anywhere)
                );







            if (model.JobTitleId.HasValue)
                items.Where(x => x.JbTitle == new JobTitle()
                {
                    Id = (long)model.JobTitleId
                });

            if (model.DepartmentId.HasValue)
                items.Where(x => x.Department == new Department()
                {
                    Id = (long)model.DepartmentId
                });




            model.Employees = items.Take(250).List();

            return View(model);
        }

        [Transaction]
        public ActionResult Card(long id)
        {
            var item = GetSession.Get<Employee>(id);

            if (item.Username == Employee.GetUsername && Employee.Current.EmployeeMessagesCount > 0)
            {
                foreach (var msg in item.EmployeeMessages.Where(x => !x.IsRead))
                {
                    msg.IsRead = true;
                    GetSession.Update(msg);
                }

                Employee.Current.EmployeeMessagesCount = 0;

                Session["employee"] = Employee.Current;
            }
            return View(item);
        }


        [Transaction]
        public ActionResult ErrorForum(long id)
        {
            var item = GetSession.Get<Employee>(id);


            return View(item);
        }

        [HttpPost, Transaction]
        public ActionResult CreateMessage(EmployeeMessage item)
        {

            item.CreatedDate = DateTime.Now;
            item.CreatedBy = new Employee
            {
                Id = GetEmployeeId
            };

            if (item.To.Id != item.CreatedBy.Id)
            {
                var employee = GetSession.Load<Employee>(item.To.Id);
                item.To = employee;

                /*	if (employee.Email.Contains("@")) {
                        new NotificationsController().Message(employee).Deliver();
                    }*/
            }
            GetSession.Save(item);

            return RedirectToAction("Card", new { item.To.Id });
        }

        [HttpPost, Transaction]
        public ActionResult UpdatePostContent(string postContent)
        {

            var item = GetSession.Get<Employee>(GetEmployeeId);

            item.PostContent = postContent;

            GetSession.Update(item);

            return RedirectToAction("Card", new { item.Id });
        }





        [HttpPost, Transaction]
        public ActionResult UpdateAlias(string alias)
        {
            var item = GetSession.Get<Employee>(GetEmployeeId);

            item.Alias = alias;

            GetSession.Update(item);

            return RedirectToAction("Card", new { item.Id });
        }






        [ChildActionOnly]
        public ActionResult BDayTicker()
        {

            var items = GetBdayEmployees();

            return View(items);
        }

        [HttpPost, Transaction]
        public ContentResult DeleteMessage(long id)
        {
            var item = GetSession.Get<EmployeeMessage>(id);

            if (item.CreatedBy == new Employee { Id = GetEmployeeId })
            {
                GetSession.Delete(item);
                return Content("ok");
            }

            return Content("not owner");
        }

        [HttpPost, Transaction]
        public ActionResult UploadPhotos(IList<HttpPostedFileBase> photos)
        {
            var employee = new Employee { Id = GetEmployeeId };

            foreach (var item in photos)
            {
                if (item != null && item.ContentLength > 0)
                {
                    var picture = Guid.NewGuid() + ".jpg";
                    var image = new WebImage(item.InputStream);

                    image.Resize(601, 601)
                        .Crop(1, 1)
                        .Save("~/public/UserFiles/employeephotos/big/" + picture, "image/jpeg")
                        .Resize(101, 101)
                        .Crop(1, 1)
                        .Save("~/public/UserFiles/employeephotos/small/" + picture, "image/jpeg");

                    GetSession.Save(new EmployeePhoto
                    {
                        Employee = employee,
                        FileName = picture
                    });
                }

            }
            return RedirectToAction("Card", new { employee.Id });
        }

        [HttpPost, Transaction]
        public ActionResult DeletePhoto(long id)
        {
            var item = GetSession.Get<EmployeePhoto>(id);

            if (item != null && item.Employee.Id == GetEmployeeId)
            {
                GetSession.Delete(item);
            }

            return RedirectToAction("Card", new { Id = GetEmployeeId });
        }

        public ActionResult ChangeInfo(bool isSent = false)
        {
            ViewBag.IsSent = isSent;
            return View();
        }

        [HttpPost]
        public ActionResult ChangeInfo(string feedback)
        {

            SendEmail(MvcApplication.Config("email.ChangeInfo", "okunevmax@gmail.com"), "בקשה לשינוי Details",
                Employee.Current.FullName +
                Environment.NewLine +
                Environment.NewLine +
                feedback);

            return RedirectToAction("ChangeInfo", new { isSent = true });
        }


        public ActionResult GetEmpTomorrowBday()
        {

            var model = GetSession.Get<Setting>((long)1);

            try
            {
                var items = GetBdayEmployeesNextDayFromToday();
                if (items.Count() == 0)
                {
                    return Content("No workers");
                }
                else
                {
                    //SendMailsToManagers(items);


                    var sum = 0;
                    string body = "<p>מחר יום הולדת לעובדים הבאים<br/></p>";

                    foreach (Employee item in items)
                    {
                        body += @"<p>שם: " + item.FirstName + ",<br/> שם משפחה: " + item.LastName + ",<br/> מחלקה: " + item.DepartmentName + ",<br/>מייל: " + item.Email + "</p>";
                        sum++;
                    }



                    var message = new MailMessage();
                    message.To.Add(new MailAddress(MvcApplication.Config("email.BDayNotificationsAdmin")));

                    //if (model.BDayEmails != "")
                    //{
                    //    string[] emails = model.BDayEmails.Split(',');
                    //    foreach (var item in emails)
                    //     {
                    //              message.CC.Add(new MailAddress(item));
                    //     }

                    //}
                    message.Subject = "BDay notification system";
                    message.IsBodyHtml = true;
                    //message.Body = model.BDayContent;
                    message.Body = body;

                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(message);
                        return Content("Sent about " + sum + "workers ");
                    }


                }

            }
            catch (Exception ex)
            {

                return Content("Error: " + ex.Message);
            }





        }

        //private void SendMailsToManagers(IList<Employee> items)
        //{


        //    foreach (var item in items)
        //    {
        //        string body = @"מחר יום הולדת ל" + item.FirstName +" "+ item.LastName;

        //        try
        //        {
        //            SendEmail(item.Manager.Email, "יום הולדת ל", body);
        //        }
        //        catch (Exception ex)
        //        {

        //            SendEmail(MvcApplication.Config("email.BDayNotificationsAdmin").ToString(), "יום הולדת ל", body);
        //            SendEmail(MvcApplication.Config("email.Feedback").ToString(), "Problem sending mail to user's manager", "Problem sending mail to user's manager due to " + ex.Message);
        //        }
        //    }
        //}




    }
}
