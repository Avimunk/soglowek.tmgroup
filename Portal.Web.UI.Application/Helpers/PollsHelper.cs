using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;

namespace Portal.Helpers
{
	public static class PollsHelper
	{
        public static MvcHtmlString ProgressBar(this HtmlHelper helper, PollItem item, int count)
        {
            float percent = 0;
            float total = item.Option1Count + item.Option2Count + item.Option3Count + item.Option4Count + item.Option5Count + item.Option6Count;

            if (count != 0)
            {
                percent = (float)count / total * 100;
            }

            return MvcHtmlString.Create(string.Format("<div class='progress-bar'><span>{0:0}%</span><div style='width: {0:0}%'></div></div>", percent));
        }
	}
}