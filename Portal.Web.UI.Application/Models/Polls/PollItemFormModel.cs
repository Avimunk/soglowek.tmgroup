using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentValidation;
using FluentValidation.Attributes;

namespace Portal.Models.Polls
{
	public class PollItemFormModel
	{
		public long Id { get; set; }
        public long Poll_Id { get; set; }
        public string Title { get; set; }
		public string Option1 { get; set; }
		public string Option2 { get; set; }
		public string Option3 { get; set; }
		public string Option4 { get; set; }
        public string Option5 { get; set; }
        public string Option6 { get; set; }
	}
}