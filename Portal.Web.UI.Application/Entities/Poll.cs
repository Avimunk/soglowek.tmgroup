using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class Poll : Entity
	{
		public Poll() {
			CreatedDate = DateTime.Now;
		}
        
		public virtual DateTime CreatedDate { get; set; }
		public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Summary { get; set; }
        public virtual bool Anonymous { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual string DisabledContent { get; set; }
        public virtual bool UserEditable { get; set; }
        public virtual bool ReadOnly { get; set; }

        public virtual IList<PollItem> PollItems { get; set; }
      
	}
}