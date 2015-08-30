using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.Jobs;
using AutoMapper;

namespace Portal.Controllers
{
	[Employee]
    public class JobsController : ApplicationController
	{
		[Admin]
		public ActionResult Index() {
            var items = GetSession.QueryOver<Job>()
				.List();
			return View(items);
		}

		[Admin]
		public ActionResult Create() {
			var model = new JobFormModel();
			return View(model);
		}

		[Admin, Transaction, HttpPost, ValidateInput(false)]
		public ActionResult Create(JobFormModel model) {

			var item = Mapper.Map<JobFormModel, Job>(model);
			GetSession.Save(item);

			return RedirectToAction("Index");
		}

		[Admin]
		public ActionResult Edit(long id) {
			var item = GetSession.Get<Job>(id);

			var model = Mapper.Map<Job, JobFormModel>(item);

			return View(model);
		}

		[Admin, Transaction, HttpPost, ValidateInput(false)]
		public ActionResult Edit(JobFormModel model) {
			if (ModelState.IsValid) {
				var item = GetSession.Get<Job>(model.Id);

				Mapper.Map<JobFormModel, Job>(model, item);

				GetSession.Update(item);

				return RedirectToAction("Index");
			}
			return View(model);
		}

		[Admin, Transaction]
		public ActionResult Destroy(long id) {
			var item = GetSession.Get<Job>(id);
			GetSession.Delete(item);
			return RedirectToAction("Index");
		}

		[ChildActionOnly]
		public ActionResult JobTicker() {
			var items = GetSession.QueryOver<Job>()
				.OrderBy(x => x.Id).Desc
				.List();
			return View(items);
		}



        public ActionResult Show(long? id)
        {
            if (id.HasValue)
            {
                var item = GetSession.QueryOver<Job>()
                    .Where(x => x.Id == id)
                    .List()
                    .FirstOrDefault();



                if (item == null)
                    return Redirect("404.aspx");

                return View(item);
            }
            return Redirect("404.aspx");
        }
	}
}
