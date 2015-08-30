using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class Forum : Entity
	{
		public Forum() {
			CreatedDate = DateTime.Now;
		}

		public virtual string Subject { get; set; }
		public virtual string Body { get; set; }
		public virtual IList<Forum> SubForums { get; set; }
		public virtual Employee CreatedBy { get; set; }
		public virtual DateTime CreatedDate { get; set; }
		public virtual Forum Parent { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual string CategoryTitle { get; set; }
        //public virtual IList<Employee> Employees { get; set; }
        public virtual long Likes { get; set; }
	}
}