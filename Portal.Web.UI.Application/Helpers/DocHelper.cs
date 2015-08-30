using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.Docs;
using AutoMapper;
using System.IO;
using NHibernate.Transform;

namespace Portal.Helpers
{
	public static class DocHelper
	{
		public static MvcHtmlString CountSubmited(this HtmlHelper helper, Doc doc) {

            string[] count = new string[] { };
            if (string.IsNullOrEmpty(doc.Submited))
            {
                return MvcHtmlString.Create("0");
            }
            else
            {
                count = doc.Submited.Split(',');

                return MvcHtmlString.Create(count.Length.ToString());
            }
			
		}



        public static MvcHtmlString CountNotSubmited(this HtmlHelper helper, Doc doc,IEnumerable<Employee> emps)
        {

            int submitted = Convert.ToInt32(CountSubmited(helper, doc));

            int allemps = emps.Count();

            int res = allemps - submitted;
            return MvcHtmlString.Create(res.ToString());

        }




	}
}