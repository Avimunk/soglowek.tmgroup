using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class DepartmentMap : ClassMap<Department>
	{
		public DepartmentMap() {
			Id(x => x.Id);

			Map(x => x.Name);

			HasMany(x => x.Employees);
		}
	}
}