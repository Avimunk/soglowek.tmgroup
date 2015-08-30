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
	public class ForumItemModel
	{
        public string Title { get; set; }
		public long? Admin { get; set; }
		public bool Active { get; set; }
        public bool isAvailableTooAll { get; set; }
	}
}