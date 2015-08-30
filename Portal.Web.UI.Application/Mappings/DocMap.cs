using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class DocMap : ClassMap<Doc>
	{
        public DocMap()
        {
			Id(x => x.Id);

			Map(x => x.Name);
			Map(x => x.Content).Length(4000);
			Map(x => x.Url);
            Map(x => x.Active);
            Map(x => x.Downloaded);
            Map(x => x.Units);
            Map(x => x.Submited);
            Map(x => x.NotSubmited);
            Map(x => x.UsersAllowed).Length(99999);
            Map(x => x.isAvailable2All);
            
		}
	}
}