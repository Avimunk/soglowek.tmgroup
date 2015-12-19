using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Models.Polls;
using Portal.Entities;
using Portal.Attributes;
using Portal.Common;
using Portal.Common.Extensions;

using AutoMapper;
using NHibernate.Linq;

namespace Portal.Controllers
{
	[Employee]
    public class PollItemsController : ApplicationController
	{
		[Admin]
		public ActionResult Index() {
            var items = GetSession.QueryOver<PollItem>()
				.List();
			return View(items);
		}

		[Admin]
		public ActionResult Create() {

            var model = new PollItemFormModel();
            model.Poll_Id = Convert.ToInt64(Url.RequestContext.RouteData.Values["id"]);
			return View(model);
		}

        [Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Create(PollItemFormModel model)
        {
            if (ModelState.IsValid)
            {
                var item = Mapper.Map<PollItemFormModel, PollItem>(model);

                item.Poll = GetSession.Get<Poll>(model.Poll_Id);

                GetSession.Save(item);
                
                return Redirect(Url.RouteUrl(new { controller = "Polls", action = "Edit", id = model.Poll_Id }) + "#items");
                //return RedirectToAction("Edit", "Polls", new { id = model.Poll_Id });
            }
            return View(model);
        }

        public int GetAvailableSlot(PollItemFormModel item)
        {
            if (String.IsNullOrEmpty(item.Option1))
            {
                return 1;
            }
            if (String.IsNullOrEmpty(item.Option2))
            {
                return 2;
            }
            if (String.IsNullOrEmpty(item.Option3))
            {
                return 3;
            }
            if (String.IsNullOrEmpty(item.Option4))
            {
                return 4;
            }
            if (String.IsNullOrEmpty(item.Option5))
            {
                return 5;
            }
            if (String.IsNullOrEmpty(item.Option6))
            {
                return 6;
            }

            return 0;
        }

		[Admin]
		public ActionResult Edit(long id) {

            var item = GetSession.Get<PollItem>(id);

            var model = Mapper.Map<PollItem, PollItemFormModel>(item);

			return View(model);
		}

		[Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Edit(PollItemFormModel model)
        {
            var item = GetSession.Get<PollItem>(model.Id);

			if (ModelState.IsValid) {
                Mapper.Map<PollItemFormModel, PollItem>(model, item);

				GetSession.Update(item);

                return Redirect(Url.RouteUrl(new { controller = "Polls", action = "Edit", id = model.Poll_Id }) + "#items");
                //return RedirectToAction("Edit", "Polls", new { id = model.Poll_Id });
			}

			return View(model);
		}

		[Admin, Transaction]
		public ActionResult Destroy(long id) {

            var item = GetSession.Get<PollItem>(id);

			GetSession.Delete(item);

            return RedirectToAction("Edit", "Polls", new { id = item.Poll.Id });
		}
	}
}
