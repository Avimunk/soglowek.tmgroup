using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class Job : Entity
	{
		public virtual string Name { get; set; }
		public virtual string Url { get; set; }
		public virtual string Content { get; set; }
	}
}