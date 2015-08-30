using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.Messages;
using AutoMapper;

namespace Portal.Controllers
{
	[Employee]
	public class MessagesController : ApplicationController
	{
		[Admin]
		public ActionResult Index() {
            var items = GetSession.QueryOver<Message>()
				.List();
			return View(items);
		}

		[Admin]
		public ActionResult Create() {
			var model = new MessageFormModel();
			return View(model);
		}

		[Admin, Transaction, HttpPost, ValidateInput(false)]
		public ActionResult Create(MessageFormModel model) {

            var item = Mapper.Map<MessageFormModel, Message>(model);
			GetSession.Save(item);

			return RedirectToAction("Index");
		}

		[Admin]
		public ActionResult Edit(long id) {
            var item = GetSession.Get<Message>(id);

            var model = Mapper.Map<Message, MessageFormModel>(item);

			return View(model);
		}

		[Admin, Transaction, HttpPost, ValidateInput(false)]
		public ActionResult Edit(MessageFormModel model) {
			if (ModelState.IsValid) {
                var item = GetSession.Get<Message>(model.Id);

                Mapper.Map<MessageFormModel, Message>(model, item);

				GetSession.Update(item);

				return RedirectToAction("Index");
			}
			return View(model);
		}

		[Admin, Transaction]
		public ActionResult Destroy(long id) {
            var item = GetSession.Get<Message>(id);
			GetSession.Delete(item);
			return RedirectToAction("Index");
		}

		[ChildActionOnly]
		public ActionResult MessagesTicker() {
            var items = GetSession.QueryOver<Message>()
				.OrderBy(x => x.Id).Desc
				.List();
			return View(items);
		}

        public ActionResult Show(long? id)
        {
            if (id.HasValue)
            {
                var item = GetSession.QueryOver<Message>()
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
