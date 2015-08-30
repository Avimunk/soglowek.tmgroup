using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using AutoMapper;
using Portal.Models.FlashBanners;
using System.IO;
using NHibernate.Linq;

namespace Portal.Controllers
{
	public class FlashBannersController : ApplicationController
	{
		[Authorize]
		public ActionResult Index() {
			var items = GetSession.QueryOver<FlashBanner>()
				.OrderBy(x => x.BannerOrder)
				.Asc
				.List();
			return View(items);
		}

		[ChildActionOnly]
		public ActionResult List() {
			var items = GetSession.QueryOver<FlashBanner>()
				.List();
			return PartialView(items);
		}

		[Authorize]
		public ActionResult Create() {
			var model = new FlashBannerFormModel();

			model.InitMembers();

			return View(model);
		}

		[Authorize, HttpPost, Transaction, ValidateInput(false)]
		public ActionResult Create(FlashBannerFormModel model, HttpPostedFileBase uploadPicture) {
			if (ModelState.IsValid) {
				var item = Mapper.Map<FlashBannerFormModel, FlashBanner>(model);

				if (uploadPicture != null && uploadPicture.ContentLength > 0) {
					if (model.TypeId == 1) {
						item.Banner += "|" + SaveFile(uploadPicture, "flashBanners", item.Banner);


					}
					else {
						item.Banner = SaveFile(uploadPicture, "flashBanners", item.Banner);
					}
				}

				int maxOrder = GetSession.Query<FlashBanner>()
					.Max(x => (int?)x.BannerOrder) ?? 0;

				item.BannerOrder = maxOrder + 1;

				GetSession.Save(item);
				return RedirectToAction("Index");
			}

			model.InitMembers();

			return View(model);
		}

		[Authorize]
		public ActionResult Edit(long id) {
			var item = GetSession.Get<FlashBanner>(id);

			var model = Mapper.Map<FlashBanner, FlashBannerFormModel>(item);

			model.InitMembers();

			return View(model);
		}

		[HttpPost, Authorize, Transaction, ValidateInput(false)]
		public ActionResult Edit(FlashBannerFormModel model, HttpPostedFileBase uploadPicture) {
			var item = GetSession.Get<FlashBanner>(model.Id);

			Mapper.Map<FlashBannerFormModel, FlashBanner>(model, item);

			if (uploadPicture != null && uploadPicture.ContentLength > 0) {
				if (model.TypeId == 1) {
					item.Banner += "|" + SaveFile(uploadPicture, "flashBanners", item.Banner);
				}
				else {
					item.Banner = SaveFile(uploadPicture, "flashBanners", item.Banner);
				}
			}

			GetSession.Update(item);

			return RedirectToAction("Index");
		}

		[Authorize, Transaction]
		public ActionResult Destroy(long id) {
			var item = GetSession.Get<FlashBanner>(id);

			GetSession.Delete(item);

			DeleteFile("flashBanners", item.Banner);

			return RedirectToAction("Index");
		}

		[Authorize, HttpPost, Transaction]
		public ActionResult BannerOrder(long id, int bannerOrder) {
			var item = GetSession.Get<FlashBanner>(id);
			item.BannerOrder = bannerOrder;

			GetSession.Update(item);

			return new EmptyResult();
		}

		[ChildActionOnly]
		public ActionResult FlashBanner() {
			var items = GetSession.QueryOver<FlashBanner>()
				.Where(x => x.IsActive)
				.OrderBy(x => x.BannerOrder)
				.Asc
				.List();

			return PartialView(items);
		}

		[Transaction, HttpPost]
		public ContentResult FlashBannerActive(long id, bool isActive) {
			var item = GetSession.Get<FlashBanner>(id);
			item.IsActive = isActive;
			GetSession.Update(item);
			return Content("ok");
		}
	}
}
