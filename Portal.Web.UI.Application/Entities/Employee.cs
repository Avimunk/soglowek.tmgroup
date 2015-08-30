using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class Employee : Entity
    {

        public virtual long Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Username { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EnglishFirstName { get; set; }
        public virtual string EnglishLastName { get; set; }
        public virtual JobTitle JbTitle { get; set; }
        public virtual long EmployeeMessagesCount { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Range { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime BDay { get; set; }
     
        public virtual Department Department { get; set; }
     
        public virtual Employee Manager { get; set; }


        public virtual string PostContent { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual string Ip { get; set; }
        public virtual string Password { get; set; }
        public virtual int StartYear { get; set; }
        public virtual string Picture { get; set; }
        public virtual int Local_id { get; set; }
        public virtual IList<Like> Likes { get; set; }
        public virtual IList<Forum> Forums { get; set; }

        public virtual IList<EmployeePhoto> EmployeePhotos { get; set; }
        public virtual IList<EmployeeMessage> EmployeeMessages { get; set; }
        public virtual IList<EmployeeMessage> SentEmployeeMessages { get; set; }

        public virtual string FullName {
            get {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }


        public virtual string DepartmentName {
            get {
                if (Department == null)
                    return "N/A";
                return Department.Name;
            }
        }

        //public virtual string ManagerName {
        //    get {
        //        if (Manager == null)
        //            return "N/A";
        //        return Manager.FullName;
        //    }
        //}

        public virtual string JobTitleName
        {
            get
            {
                if (JbTitle == null)
                    return "אין";
                return JbTitle.Name;
            }
        }


        public static Employee Current {
            get {
                var item = HttpContext.Current.Session["employee"] as Employee;
                return item;
            }
        }

        public static string GetUsername
        {
            get
            {
                // var item
                //var item = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //var item = HttpContext.Current.User.Identity.Name;
                //var item = httpconte
                //var item = HttpContext.Current.Request.Cookies["user"].Value;
                // var item = "alex1";

                var item = HttpContext.Current.User.Identity.Name;

                string ssoConfig = MvcApplication.Config("sso.enabled");
                if (!String.IsNullOrEmpty(ssoConfig))
                {
                    bool ssoEnabled = bool.Parse(ssoConfig);
                    if (ssoEnabled)
                    {
                        if (HttpContext.Current != null && !String.IsNullOrWhiteSpace(HttpContext.Current.Request.Cookies["user_name"].Value))
                        {
                            item = HttpContext.Current.Request.Cookies["user_name"].Value;
                        }
                    }
                }

                //var item = HttpContext.Current.Request.Cookies["user_name"].Value;
                if (item.Contains("\\"))
                {
                    item = item.Split('\\')[1];
                }

                return item;
            }
        }

        public static bool IsInternel {
            get {
                return MvcApplication.Config("page.InternalIp") == HttpContext.Current.Request.UserHostAddress;
            }
        }
    }
}