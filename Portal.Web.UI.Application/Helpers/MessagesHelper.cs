using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;

namespace Portal.Helpers
{
	public static class MessagesHelper
	{
        public static MvcHtmlString MessageUrl(this HtmlHelper helper, Message message)
        {
			if (string.IsNullOrEmpty(message.Url))
				return MvcHtmlString.Create(message.Name);

			return MvcHtmlString.Create(string.Format("<a href='{0}' target='_blank'>{1}</a>", 
				message.Url,
				message.Name)
			);
		}
	}
}