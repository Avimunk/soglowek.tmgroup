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
            Map(x => x.LayoutDirection);
            Map(x => x.RequireDepartment);
            Map(x => x.RequireManager);
            Map(x => x.WindowTarget);
            Map(x => x.StartCaption);
            Map(x => x.EndCaption);
            Map(x => x.IrrelevantCaption);
            Map(x => x.LCID);

            References(x => x.Object);
            References(x => x.AnonymousUser);
            
            HasMany(x => x.PollItems).KeyColumn("Poll_Id");
		}
	}
}