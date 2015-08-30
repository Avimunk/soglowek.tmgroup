using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class JobMap : ClassMap<Job>
	{
        public JobMap()
        {
			Id(x => x.Id);

			Map(x => x.Name);
			Map(x => x.Content).Length(4000);
			Map(x => x.Url);
		}
	}
}