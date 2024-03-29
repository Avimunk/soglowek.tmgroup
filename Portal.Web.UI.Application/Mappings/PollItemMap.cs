﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using Portal.Entities;

namespace Portal.Mappings
{
	public class PollItemMap : ClassMap<PollItem>
	{
        public PollItemMap()
        {
			Id(x => x.Id);

            Map(x => x.Type).CustomType<int>();
            Map(x => x.Heading);
            Map(x => x.Title);
            Map(x => x.Caption);
            Map(x => x.Description);
            Map(x => x.MinimumValue);
            Map(x => x.MinimumText);
            Map(x => x.MaximumValue);
            Map(x => x.MaximumText);
            Map(x => x.Required);
            Map(x => x.Option1);
			Map(x => x.Option2);
			Map(x => x.Option3);
			Map(x => x.Option4);
            Map(x => x.Option5);
            Map(x => x.Option6);
			Map(x => x.Option1Count);
			Map(x => x.Option2Count);
			Map(x => x.Option3Count);
			Map(x => x.Option4Count);
            Map(x => x.Option5Count);
            Map(x => x.Option6Count);
            Map(x => x.LCID);

            References(x => x.Poll);

            HasMany(x => x.EmployeePollItems).KeyColumn("PollItem_Id");
        }
	}
}