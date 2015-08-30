using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using System.Web.Mvc;
using NHibernate;
using NHibernate.Linq;
using System.Web.Helpers;

namespace Portal.Models.Employees
{
    public class EmployeeFormModel
    {
        public void InitMembers(ISession session) {
            Years = Enumerable.Range(1985, DateTime.Now.Year+1 - 1985)
                .Select(x => new SelectListItem() {
                    Value = x.ToString(),
                    Text = x.ToString(),
                });

         

            var jobs = session.Query<JobTitle>().ToList();
            JobTitles = new List<SelectListItem>() { };
            foreach (var item in jobs) {
                JobTitles.Add(new SelectListItem {
                    Value = item.Name,
                    Text = item.Name
                });
            }

            var departments = session.Query<Department>().ToList();
            Departments = new List<SelectListItem>() { };
            foreach (var item in departments)
            {
                Departments.Add(new SelectListItem
                {
                    Value = item.Name,
                    Text = item.Name
                });
            }




            AllManagers = session.Query<Employee>()
              .OrderBy(x => x.Manager)
              .Select(x => new Employee
              {
                  Manager = x.Manager,
                  Id = x.Id
              })
              .ToList();

        }
        public long Id { get; set; }
        public bool IsActive { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishLastName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Range { get; set; }
        public string Email { get; set; }
        public int StartYear { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
        public DateTime BDay { get; set; }
        public string PostContent { get; set; }
        public bool IsAdmin { get; set; }
        public string Ip { get; set; }
        public string Password { get; set; }
       
        public Employee Manager { get; set; }
        public IList<Employee> AllManagers { get; set; }


        public JobTitle JbTitle { get; set; }
        public IList<SelectListItem> JobTitles { get; set; }

        public Department Department { get; set; }
        public IList<SelectListItem> Departments { get; set; }

       

        public string Picture { get; set; }
        public HttpPostedFileBase _uploadPicture;
        public HttpPostedFileBase UploadPicture {
            get {
                return _uploadPicture;
            }
            set {
                _uploadPicture = value;
                if (_uploadPicture != null) {
                    var picture = Guid.NewGuid() + ".jpg";
                    var image = new WebImage(_uploadPicture.InputStream);
                    image.Save("~/public/userfiles/employees/" + picture, "image/jpeg");

                    Picture = picture;
                }
            }
        }
    }
}