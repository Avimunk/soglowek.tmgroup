using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.StreamLink;
using AutoMapper;

namespace Portal.Controllers
{
	[Employee]
    public class StreamLinkController : ApplicationController
	{
		[Admin]
		public ActionResult Index() {
            var items = GetSession.QueryOver<StreamLink>()
				.List();
			return View(items);
		}

		[Admin]
		public ActionResult Create() {
            var model = new StreamLinkFormModel();
			return View(model);
		}

		[Admin, Transaction, HttpPost, ValidateInput(false)]
        public ActionResult Create(StreamLinkFormModel model)
        {

            var item = Mapper.Map<StreamLinkFormModel, StreamLink>(model);
			GetSession.Save(item);

			return RedirectToAction("Index");
		}

		[Admin]
		public ActionResult Edit(long id) {
            var item = GetSession.Get<StreamLink>(id);

            var model = Mapper.Map<StreamLink, StreamLinkFormModel>(item);

			return View(model);
		}

		[Admin, Transaction, HttpPost, ValidateInput(false)]
        public ActionResult Edit(StreamLinkFormModel model)
        {
			if (ModelState.IsValid) {
                var item = GetSession.Get<StreamLink>(model.Id);

                Mapper.Map<StreamLinkFormModel, StreamLink>(model, item);

				GetSession.Update(item);

				return RedirectToAction("Index");
			}
			return View(model);
		}

		[Admin, Transaction]
		public ActionResult Destroy(long id) {
            var item = GetSession.Get<StreamLink>(id);
			GetSession.Delete(item);
			return RedirectToAction("Index");
		}

		
        public ActionResult StreamLinkTicker()
        {
            var items = GetSession.QueryOver<StreamLink>()
				.OrderBy(x => x.Id).Desc
				.List();
			return View(items);
		}

        public ActionResult Show(long? id)
        {
            if (id.HasValue)
            {
                var item = GetSession.QueryOver<StreamLink>()
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
