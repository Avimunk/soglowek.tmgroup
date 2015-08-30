using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;

namespace Kidum.Helpers
{
    public static class FlashBannersHelper
    {
        public static MvcHtmlString PrintFlashBanner(this HtmlHelper helper, FlashBanner flashBanner)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            string[] banners = flashBanner.Banner.Split('|').Where(x => x.Trim().Length > 0).ToArray();
            string banner = string.Empty;
            foreach (string item in banners)
            {
                string img = string.Format("<img src='{0}' width='1008' height='285' />",
                    "~/public/uploads/flashBanners/" + item);
            }
            return MvcHtmlString.Create(banner);
        }
    }
}