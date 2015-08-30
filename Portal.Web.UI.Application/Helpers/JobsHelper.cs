using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;

namespace Portal.Helpers
{
	public static class JobsHelper
	{
        public static MvcHtmlString JobsUrl(this HtmlHelper helper, Job job)
        {
            if (string.IsNullOrEmpty(job.Url))
                return MvcHtmlString.Create(job.Name);

			return MvcHtmlString.Create(string.Format("<a href='{0}' target='_blank'>{1}</a>",
                job.Url,
                job.Name)
			);
		}
	}
}