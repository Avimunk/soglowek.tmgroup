using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

using Portal.Common.Extensions;
using System.ComponentModel;
using Portal.Helpers;

namespace Portal.Entities
{
    public enum QuestionType
    {
        Multiple = 1,
        Open = 2,
        Range = 3
    }
    public class PollItem : Entity
	{
		public PollItem() {
		}

        public virtual Poll Poll { get; set; }
        public virtual QuestionType Type { get; set; }
        public virtual string Heading { get; set; }
        public virtual string Caption { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual int MinimumValue { get; set; }
        public virtual string MinimumText { get; set; }
        public virtual int MaximumValue { get; set; }
        public virtual string MaximumText { get; set; }
        public virtual bool Required { get; set; }
		public virtual string Option1 { get; set; }
		public virtual string Option2 { get; set; }
		public virtual string Option3 { get; set; }
        public virtual string Option4 { get; set; }
        public virtual string Option5 { get; set; }
		public virtual string Option6 { get; set; }
		public virtual int Option1Count { get; set; }
		public virtual int Option2Count { get; set; }
		public virtual int Option3Count { get; set; }
		public virtual int Option4Count { get; set; }
        public virtual int Option5Count { get; set; }
        public virtual int Option6Count { get; set; }
        public virtual int LCID { get; set; }

        public virtual IList<EmployeePollItem> EmployeePollItems { get; set; }
      
	}
}