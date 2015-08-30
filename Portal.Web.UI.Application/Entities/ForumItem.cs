using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class ForumItem : Entity
	{
        public virtual string Title { get; set; }
        public virtual Employee Admin { get; set; }
        public virtual bool Active { get; set; }
        public virtual string ForumUsers { get; set; }
        public virtual bool isAvailableTooAll { get; set; }
	}
}