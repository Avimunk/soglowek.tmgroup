using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class EmployeePollItem : Entity
	{
        public EmployeePollItem()
        {
		}

        public virtual Employee Employee { get; set; }
        public virtual PollItem PollItem { get; set; }
		public virtual int Option1Count { get; set; }
		public virtual int Option2Count { get; set; }
		public virtual int Option3Count { get; set; }
		public virtual int Option4Count { get; set; }
        public virtual int Option5Count { get; set; }
        public virtual int Option6Count { get; set; }
      
	}
}