using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class EmployeeMessage : Entity
	{
		public virtual string Message { get; set; }
		public virtual bool IsRead { get; set; }
		public virtual DateTime CreatedDate { get; set; }
		public virtual Employee CreatedBy { get; set; }
		public virtual Employee To { get; set; }
	}
}