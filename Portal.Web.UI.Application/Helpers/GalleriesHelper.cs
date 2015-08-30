using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using System.IO;

namespace Portal.Helpers {
    public static class GalleriesHelper {
        public static MvcHtmlString GalleryPicture(this HtmlHelper helper, Gallery gallery) {
            return helper.GalleryPicture(gallery, PictureSize.Large);
        }

        public static MvcHtmlString GalleryPicture(this HtmlHelper helper, Gallery gallery, PictureSize size) {
            if (string.IsNullOrEmpty(gallery.DefaultPhoto))
                return MvcHtmlString.Empty;
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            return MvcHtmlString.Create(string.Format("<img  width='200' src='{0}{3}/{4}/{2}' title='{1}'/>", url.Content("~/public/userfiles/galleries/"), gallery.Title, gallery.DefaultPhoto, size, gallery.Id));
        }
    }
}