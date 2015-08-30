using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class ForumMap : ClassMap<Forum>
	{
		public ForumMap() {
			Id(x => x.Id);

			Map(x => x.Subject);
			Map(x => x.CreatedDate);
			Map(x => x.Body).Length(4001); ;
            Map(x => x.CategoryId);
            Map(x => x.CategoryTitle);
			References(x => x.Parent);
			References(x => x.CreatedBy);
            Map(x => x.Likes);
			HasMany(x => x.SubForums).OrderBy("CreatedDate").KeyColumn("Parent_Id").Cascade.AllDeleteOrphan();
		}
	}
}