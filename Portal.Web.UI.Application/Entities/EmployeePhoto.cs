using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class EmployeePhoto : Entity
	{
		public virtual string FileName { get; set; }
		public virtual Employee Employee { get; set; }
	}
}