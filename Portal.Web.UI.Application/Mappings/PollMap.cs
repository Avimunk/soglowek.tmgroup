using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using Portal.Entities;

namespace Portal.Mappings
{
	public class PollMap : ClassMap<Poll>
	{
		public PollMap() {
			Id(x => x.Id);

            Map(x => x.CreatedDate);
            Map(x => x.Title);
            Map(x => x.Description);
            Map(x => x.Summary);
            Map(x => x.Anonymous);
            Map(x => x.Enabled);
            Map(x => x.DisabledContent);
            Map(x => x.UserEditable);

            HasMany(x => x.PollItems).KeyColumn("Poll_Id");
              
		}
	}
}