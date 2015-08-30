using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;

namespace Portal.Helpers
{
	public static class EmployeePhotosHelper
	{
		public static MvcHtmlString EmplloyeePhoto(this HtmlHelper helper, EmployeePhoto photo) {
			if (photo == null)
				return MvcHtmlString.Empty;

			var url = new UrlHelper(helper.ViewContext.RequestContext);

			return MvcHtmlString.Create(string.Format("<a rel='gallery' class='employeephoto-photo' href='{0}'><img src='{1}'/></a>",
				url.Content("~/public/UserFiles/employeephotos/big/" + photo.FileName),
				url.Content("~/public/UserFiles/employeephotos/small/" + photo.FileName)
			));
		}

        public static string PictureUrl(this UrlHelper helper, Employee employee) {
            if (string.IsNullOrEmpty(employee.Picture))
                return "";

            return helper.Content("~/public/userfiles/employees/" + employee.Picture);
        }
	}
}