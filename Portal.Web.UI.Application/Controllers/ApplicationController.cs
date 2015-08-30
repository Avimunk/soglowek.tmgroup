using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;

using System.Configuration;
using System.IO;
using System.Web.Helpers;
using System.Net.Mail;
using Portal.Entities;
using NHibernate.Transform;

namespace Portal.Controllers
{
    public abstract class ApplicationController : Controller
    {
        private ISession session;
        public ISession GetSession {
            get {
                if (session == null) {
                    session = NHibernateHelper.GetCurrentSession();
                }
                return session;
            }
        }

        public void SendEmail(string to, string subject, string body) {
            var client = new SmtpClient();

            var msg = new MailMessage();

            msg.To.Add(to);
            msg.Subject = "פורטל: " + subject;
            msg.Body = body;

            client.Send(msg);
        }





        public void SendForumEmail(string to, string subject, string body)
        {
            var client = new SmtpClient();
            
            var msg = new MailMessage();

            msg.To.Add(to);
            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = body;

            client.Send(msg);
        }


        private long _employeeId = 0;
        public long GetEmployeeId {
            get {
                if (_employeeId == 0)
                    _employeeId = (Session["employee"] as Employee).Id;
                return _employeeId;
            }
        }

        protected IList<Employee> GetBdayEmployees() {
            return GetSession.CreateQuery("from Employee where IsActive=:IsActive and month(BDay)=:Month and day(BDay)=:Day")
                .SetBoolean("IsActive", true)
                .SetInt32("Month", DateTime.Now.Month)
                .SetInt32("Day", DateTime.Now.Day)
                .List<Employee>();
        }



        protected IList<Employee> GetBdayEmployeesNextDayFromToday()
        {
          

            return GetSession.CreateQuery("from Employee where IsActive=:IsActive and month(BDay)=:Month and day(BDay)=:Day")
                .SetBoolean("IsActive", true)
                .SetInt32("Month", DateTime.Now.Month)
                 .SetInt32("Day", DateTime.Now.AddDays(1).Day)
                .List<Employee>();
        }


        public string SaveFile(HttpPostedFileBase uploadPicture, string folder, string current)
        {
            if (uploadPicture != null && uploadPicture.ContentLength > 0)
            {
                string path = Server.MapPath("~/Public/UserFiles/" + folder);
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(uploadPicture.FileName);
                uploadPicture.SaveAs(
                    Path.Combine(path, filename)
                );
                current = filename;
            }
            return current;
        }

        public void DeleteFile(string folder, string current)
        {
            if (!string.IsNullOrEmpty(current))
            {

                var toDelete = current.Split('|');

                foreach (string item in toDelete)
                {
                    string path = Server.MapPath("~/Public/UserFiles/" + folder + "/" + item);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

            }
        }
    }
}
