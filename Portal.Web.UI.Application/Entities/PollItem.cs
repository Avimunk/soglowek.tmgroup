using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class PollItem : Entity
	{
		public PollItem() {
		}

        public virtual Poll Poll { get; set; }
        public virtual string Title { get; set; }
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

        public virtual IList<EmployeePollItem> EmployeePollItems { get; set; }
      
	}
}