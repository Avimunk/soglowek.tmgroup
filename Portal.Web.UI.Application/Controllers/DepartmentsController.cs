using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Models.Departments;
using Portal.Entities;
using Portal.Attributes;
using AutoMapper;
using NHibernate.Linq;

using Portal.Helpers;
using Portal.Web.UI;

namespace Portal.Controllers
{
	[Employee]
	public class DepartmentsController : ApplicationController
	{
		[Admin]
		public ActionResult Index() {
			var items = GetSession.QueryOver<Department>()
				.List();
			return View(items);
		}

        public ActionResult DropDownList()
        {
            var items = GetSession.QueryOver<Department>()
                .OrderBy(d => d.Name).Asc
                .List();

            var listItems = new List<SelectListItem>();

            foreach (Department department in items)
            {
                listItems.Add(new SelectListItem()
                {
                    Value = department.Id.ToString(),
                    Text = department.Name
                });
            }

            return View(items);
        }

		[Admin]
		public ActionResult Create() {
            var model = new DepartmentFormModel();
			return View(model);
		}

		[Admin, HttpPost, Transaction]
		public ActionResult Create(DepartmentFormModel model) {

			if (ModelState.IsValid) {
                var item = Mapper.Map<DepartmentFormModel, Department>(model);

				GetSession.Save(item);

				return RedirectToAction("Index");
			}

			return View(model);
		}


		[Admin]
		public ActionResult Edit(long id) {
			var item = GetSession.Get<Department>(id);
			var model = Mapper.Map<Department, DepartmentFormModel>(item);

			return View(model);
		}

		[Admin, HttpPost, Transaction]
		public ActionResult Edit(DepartmentFormModel model) {
			var item = GetSession.Get<Department>(model.Id);
            
			if (ModelState.IsValid) {
				Mapper.Map<DepartmentFormModel, Department>(model, item);

				GetSession.Update(item);

				return RedirectToAction("Index");
			}

			return View(model);
		}

		[Admin, Transaction]
		public ActionResult Destroy(long id) {

			var item = GetSession.Get<Department>(id);

			GetSession.Delete(item);

			return RedirectToAction("Index");
		}

		public ActionResult Archive() {
			var items = GetSession.QueryOver<Department>()
				.Skip(1)
				.List();
			return View(items);
		}

        //public ActionResult Show(int id) {
        //    //if (Session["optionResult" + DateTime.Now.Date.ToShortDateString()] !=null)
        //   // {
        //    IEnumerable<Employee> items = GetSession.QueryOver<Employee>().List();



        //        var item = GetSession.QueryOver<Department>()
        //            .Where(x => x.Id == id)
        //                .SingleOrDefault();
        //        ViewBag.EmployeesTree = EmployeesHelper.GetEmployeesTree(item, items);
        //        return View(item);
        //   // }
        //   // else
        //   // {

        //    //    return RedirectToAction("DepartmentResult");
        //   // }
        //}

		[HttpPost, Transaction]
        public ActionResult Show(List<int> optionResult)
        {
           

            var Departments = GetSession.QueryOver<Department>().Take(optionResult.Count).List();
           
            for (int i = 0; i < optionResult.Count; i++)
            {
                GetSession.CreateSQLQuery(
              string.Format("Update {0}Department set Option{1}Count=Option{1}Count+1 where Id={2}",
                  MvcApplication.Config("table.Prefix"),
                  optionResult[i],
                  Departments[i].Id
              )
          ).ExecuteUpdate();
                Session["optionResult" + DateTime.Now.Date.ToShortDateString()] = "done";
            }
           
            return RedirectToAction("DepartmentResult");
			
		}

		public ActionResult DepartmentResult() {
            var items = GetSession.QueryOver<Department>()
                 .List();
            return View(items);
		}
	}
}
