using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using System.Web.Mvc;
using Portal.Entities;

namespace Portal.Models.Pages
{
    public class PermissionsFormModel
    {
        public void InitMembers(ISession session)
        {


            JobTitles = session.QueryOver<JobTitle>()
                .OrderBy(x => x.Name).Asc
                .List()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

            Departments = session.QueryOver<Department>()
                .OrderBy(x => x.Name).Asc
                .List()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });



            Employees = session.QueryOver<Employee>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.FirstName).Asc
                .OrderBy(x => x.LastName).Asc
                .List()
                .Select(x => new SelectListItem()
                {
                    Text = x.FullName,
                    Value = x.Id.ToString()
                });



            if (!string.IsNullOrEmpty(JobTitle))
                JobTitleId = JobTitle.Split(',').Select(x => long.Parse(x)).ToArray();



            if (!string.IsNullOrEmpty(Department))
                DepartmentId = Department.Split(',').Select(x => long.Parse(x)).ToArray();

            if (!string.IsNullOrEmpty(Employee))
                EmployeeId = Employee.Split(',').Select(x => long.Parse(x)).ToArray();
        }


        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> JobTitles { get; set; }

        public IEnumerable<SelectListItem> Employees { get; set; }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }

        public long[] JobTitleId { get; set; }

        public long[] DepartmentId { get; set; }
        public long[] EmployeeId { get; set; }
        public bool IsInternal { get; set; }

        public int SectionId { get; set; }
        public string Employee { get; set; }
        public string JobTitle { get; set; }

        public string Department { get; set; }

    }
}