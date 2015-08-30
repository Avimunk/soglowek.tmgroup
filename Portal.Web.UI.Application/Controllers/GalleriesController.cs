using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using Portal.Models.Galleries;
using Portal.Attributes;
using AutoMapper;

namespace Portal.Controllers {

      [Employee]
    public class GalleriesController : ApplicationController {
        [Authorize]
        public ActionResult Index() {
            var items = GetSession.QueryOver<Gallery>()
                .OrderBy(x => x.Date).Desc
                .List();
            return View(items);
        }

        [Authorize]
        public ActionResult Create() {
            var model = new GalleryFormModel() {
                Id = Guid.NewGuid(),
                Date = DateTime.Now
            };
            return View(model);
        }


        [Authorize, HttpPost, Transaction]
        public ActionResult Create(GalleryFormModel model) {

            var item = Mapper.Map<GalleryFormModel, Gallery>(model);
            GetSession.Save(item);

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Edit(Guid id) {
            var item = GetSession.Get<Gallery>(id);

            var model = Mapper.Map<Gallery, GalleryFormModel>(item);

            return View(model);
        }

        [HttpPost, Authorize, Transaction]
        public ActionResult Edit(GalleryFormModel model) {
            var item = GetSession.Get<Gallery>(model.Id);

            Mapper.Map<GalleryFormModel, Gallery>(model, item);
            GetSession.Update(item);

            return RedirectToAction("Index");
        }


        [Authorize, Transaction]
        public ActionResult Destroy(Guid id) {
            var item = GetSession.Get<Gallery>(id);
            GetSession.Delete(item);
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult GalleriesBox() {
            var items = GetSession.QueryOver<Gallery>()
                .OrderBy(x => x.Id).Desc
                .Take(6)
                .List();

            return PartialView(items);
        }

        public ActionResult Show(Guid id) {
            var item = GetSession.Get<Gallery>(id);
            return View(item);
        }

        public ActionResult List() {
            var setting = GetSession.Get<Setting>((long)1);

            string host = HttpContext.ApplicationInstance.Request.Url.Host;

            if (setting != null)
                ViewBag.GalleryBody = setting.GalleryBody;

            var items = GetSession.QueryOver<Gallery>()
                .OrderBy(x => x.Date).Desc
                .List();
            return View(items);
        }

    }
}
