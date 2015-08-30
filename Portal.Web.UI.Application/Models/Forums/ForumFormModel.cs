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
	[Validator(typeof(ForumFormValidator))]
	public class ForumFormModel
	{

        //public void InitMembers(ISession session)
        //{


        //    Likes = session.Query<Forum>().Count();


        //}  
       
      
        public long Likes { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
        public int CategoryId { get; set; }
		public long? ParentId { get; set; }
		public string ParentSubject { get; set; }
        public string CategoryTitle { get; set; }

	}

	public class ForumFormValidator : AbstractValidator<ForumFormModel>
	{
		public ForumFormValidator() {


			RuleFor(x => x.Body).NotEmpty();
		}
	}
}