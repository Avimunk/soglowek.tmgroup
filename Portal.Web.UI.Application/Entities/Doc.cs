using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class Doc : Entity
	{
		public virtual string Name { get; set; }
        public virtual string Content { get; set; }
		public virtual string Url { get; set; }
        public virtual string Units { get; set; }
        public virtual string UsersAllowed { get; set; }
        public virtual bool Active { get; set; }
		public virtual string Downloaded { get; set; }
        public virtual string Submited { get; set; }
        public virtual string NotSubmited { get; set; }
        public virtual bool isAvailable2All { get; set; }
	}
}