using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class EmployeePhotoMap : ClassMap<EmployeePhoto>
	{
		public EmployeePhotoMap() {
			Id(x => x.Id);

			Map(x => x.FileName);

			References(x => x.Employee);
		}
	}
}