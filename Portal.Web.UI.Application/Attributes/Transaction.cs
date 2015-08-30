using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Attributes
{
	public class TransactionAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			NHibernateHelper.BeginTransaction();

			base.OnActionExecuting(filterContext);
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext) {
			try {
				if ((filterContext.Exception != null) && (!filterContext.ExceptionHandled)) {
					NHibernateHelper.RollbackTransaction();
				}
				else {
					NHibernateHelper.CommitTransaction();
				}
			}
			finally {
				NHibernateHelper.DisposeSession();
			}

			base.OnResultExecuted(filterContext);
		}
	}
}