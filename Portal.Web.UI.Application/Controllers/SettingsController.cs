using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using Portal.Attributes;
using System.Web.Helpers;

namespace Portal.Controllers
{
	[Authorize]
	public class SettingsController : ApplicationController
	{
		public ActionResult Edit() {
			var model = GetSession.Get<Setting>((long)1);
			return View(model);
		}

		[HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Edit(long id, HttpPostedFileBase boxFile, 
            HttpPostedFileBase bdate,
            HttpPostedFileBase box1, 
            HttpPostedFileBase box2, 
            HttpPostedFileBase box3,
            HttpPostedFileBase box4)
        {
			
            var item = GetSession.Get<Setting>(id);
            var homePath = Server.MapPath("~/public/userfiles/home/boximage.jpg");

            if (boxFile != null)
            {
                boxFile.SaveAs(string.Format(homePath));
            }

            homePath = Server.MapPath("~/public/userfiles/home/bdate.jpg");
            if (bdate != null)
            {
                bdate.SaveAs(string.Format(homePath));
            }

            homePath = Server.MapPath("~/public/userfiles/home/box1.jpg");
            if (box1 != null)
            {
                var image = new WebImage(box1.InputStream);
                image.Resize(500,212).Crop(1, 1).Save(string.Format(homePath));
            }

            homePath = Server.MapPath("~/public/userfiles/home/box2.jpg");
            if (box2 != null)
            {

                var image = new WebImage(box2.InputStream);
                image.Resize(500, 212).Crop(1, 1).Save(string.Format(homePath));
               
            }

            homePath = Server.MapPath("~/public/userfiles/home/box3.jpg");
            if (box3 != null)
            {
                var image = new WebImage(box3.InputStream);
                image.Resize(500, 212).Crop(1, 1).Save(string.Format(homePath));
            }

            homePath = Server.MapPath("~/public/userfiles/home/box4.jpg");
            if (box4 != null)
            {
                var image = new WebImage(box4.InputStream);
                image.Resize(500, 212).Crop(1, 1).Save(string.Format(homePath));
            }



			UpdateModel(item);

			GetSession.Update(item);

			return RedirectToAction("Edit");
		}



        [Transaction, HttpPost]
        public ContentResult bdaymailsIsAllow(bool isActive)
        {
            var model = GetSession.Get<Setting>((long)1);
            model.BDayEmailsAllow = isActive;
            GetSession.Update(model);
            return Content("ok");
        }

	}
}
