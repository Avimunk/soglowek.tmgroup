using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Attributes;
using Portal.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Portal.Models.Forums
{
	public class ForumUsersModel
	{

		public long? User { get; set; }
        public long? Forum { get; set; }
	}
}