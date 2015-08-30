using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class EmployeeMessageMap : ClassMap<EmployeeMessage>
	{
		public EmployeeMessageMap() {
			Id(x => x.Id);

			Map(x => x.Message);
			Map(x => x.CreatedDate);
			Map(x => x.IsRead);

			References(x => x.CreatedBy);
			References(x => x.To);
		}
	}
}