using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentValidation;
using FluentValidation.Attributes;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Portal.Common;
using Portal.Common.Extensions;

namespace Portal.Models.Polls
{
	public class PollItemFormModel
	{
		public long Id { get; set; }
        public long Poll_Id { get; set; }
        public QuestionType Type { get; set; }
        public string Heading { get; set; }
        public string Caption { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MinimumValue { get; set; }
        public string MinimumText { get; set; }
        public int MaximumValue { get; set; }
        public string MaximumText { get; set; }
        public bool Required { get; set; }
		public string Option1 { get; set; }
		public string Option2 { get; set; }
		public string Option3 { get; set; }
		public string Option4 { get; set; }
        public string Option5 { get; set; }
        public string Option6 { get; set; }

        public int LCID { get; set; }

        //public SelectList QuestionTypes { get { return Type.ToSelectList(); } }
        //@Html.DropDownList("MyType", EnumHelper.GetSelectList(typeof(MyType)), "Select My Type", new { @class = "form-control" })
	}
}