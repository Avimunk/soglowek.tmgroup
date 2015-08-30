using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Web.Helpers;
using MvcContrib.UI.Pager;
using MvcContrib.Pagination;
using System.Web.Routing;

namespace Portal.Helpers
{
    public static class ApplicationHelper
    {
        private static object lockObject = new object();

        public static MvcHtmlString Css(this HtmlHelper helper, params string[] names) {
            var html = new StringBuilder();

            foreach (string key in names) {
                html.AppendLine(
                    string.Format("<link href='{0}.css?v={1}' rel='stylesheet' type='text/css' />",
                       new UrlHelper(helper.ViewContext.RequestContext).Content("~/public/css/" + key),
                       helper.Version()
                   )
               );
            }

            return MvcHtmlString.Create(html.ToString());
        }

        public static string Version(this HtmlHelper helper) {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public static MvcHtmlString Script(this HtmlHelper helper, params string[] names) {
            var html = new StringBuilder();

            foreach (string key in names) {
                html.AppendLine(
                    string.Format("<script type='text/javascript' src='{0}.js?v={1}'></script>",
                        new UrlHelper(helper.ViewContext.RequestContext).Content("~/public/js/" + key),
                        helper.Version()
                    )
                );
            }

            return MvcHtmlString.Create(html.ToString());
        }

        public static MvcHtmlString SetBr(this string helper) {
            if (string.IsNullOrEmpty(helper))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(helper.Replace("\r\n", "<br />").Replace("\n", "<br />"));
        }

        public static MvcHtmlString GridIconLink(this UrlHelper helper, string text, string action, string controller, object routeValues, string classes) {
            return MvcHtmlString.Create(
                string.Format("<a title='{0}' href='{1}'><img class='icon {2}' src='{3}' /></a>",
                    text,
                    helper.Action(action, controller, routeValues),
                    classes,
                    helper.Content("~/public/images/pix.gif")
                )
            );

        }

        public static string GridCounter(this HtmlHelper target, ref int counter) {
            return string.Format("{0}.", counter++);
        }

        public static WebImage CreateThumbnail(this WebImage image) {
            var width = image.Width;
            var height = image.Height;

            if (width > height) {
                var leftRightCrop = (width - height) / 2;
                image.Crop(0, leftRightCrop, 0, leftRightCrop);
            }
            else if (height > width) {
                var topBottomCrop = (height - width) / 2;
                image.Crop(topBottomCrop, 0, topBottomCrop, 0);
            }

            return image;
        }

        public static string ToNiceDateTime(this DateTime target) {
            var ts = new TimeSpan(DateTime.Now.Ticks - target.Ticks);
            double delta = ts.TotalSeconds;

            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            if (delta < 1 * MINUTE) {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 2 * MINUTE) {
                return "a minute ago";
            }
            if (delta < 45 * MINUTE) {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 90 * MINUTE) {
                return "an hour ago";
            }
            if (delta < 24 * HOUR) {
                return ts.Hours + " hours ago";
            }
            if (delta < 48 * HOUR) {
                return "yesterday";
            }
            if (delta < 30 * DAY) {
                return ts.Days + " days ago";
            }
            if (delta < 12 * MONTH) {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        public static MvcHtmlString ToEmail(this string email) {
            if (string.IsNullOrEmpty(email))
                return MvcHtmlString.Create("N/A");

            return MvcHtmlString.Create(string.Format("<a href='mailto:{0}'>{0}</a>", email));
        }

        public static MvcHtmlString GalleryImage(this HtmlHelper helper, string galleryImage) {
            if (string.IsNullOrEmpty(galleryImage))
                return MvcHtmlString.Empty;

            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var path = "~/public/UserFiles/gallery/" + HttpUtility.JavaScriptStringEncode(galleryImage);

            galleryImage = url.Content(path);

            if (galleryImage.EndsWith(".swf", StringComparison.OrdinalIgnoreCase))
                return MvcHtmlString.Create(string.Format("<embed wmode='opaque' style='display: block' width='512' height='300' src='{0}'></embed>", galleryImage));

            lock (lockObject) {
                var image = new WebImage(path);

                if (image.Width > 512 || image.Height > 300) {
                    var newImage = image.Clone();

                    newImage.Resize(512, 300);

                    if (newImage.Height < 300) {
                        image.Resize(image.Width, 300);
                    }
                    else {
                        image.Resize(512, image.Height);
                    }

                    image.Save();
                }
            }

            return MvcHtmlString.Create(string.Format("<div class='gallery-image' style=\"background-image: url('{0}')\"></div>", galleryImage));


        }

        //public static Pager Pager(this HtmlHelper helper, IPagination pagination, bool isLocalize) {
        //    return new Pager(pagination, helper.ViewContext)
        //        .Format("מציג {0} - {1} מתוך {2} ")
        //        .SingleFormat("מציג {0} מתוך {1} ")
        //        .Last("אחרון")
        //        .First("ראשון")
        //        .Next("הבא")
        //        .Previous("הקודם")
        //    ;
        //}

        public static MvcHtmlString Image(this HtmlHelper target, string path) {
            return target.Image(path, null);
        }

        public static MvcHtmlString Image(this HtmlHelper target, string path, object htmlAttributes) {
            var url = new UrlHelper(target.ViewContext.RequestContext);

            var tag = new TagBuilder("img");

            if (!path.StartsWith("http"))
                path = url.Content("~/Public/images/" + path + "?v=" + target.Version());


            tag.MergeAttribute("src", path);
            tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.SelfClosing));
        }


    }

    public enum PictureSize
    {
        Small,
        Medium,
        Large
    }
}