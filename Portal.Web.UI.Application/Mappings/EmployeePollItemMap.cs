using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;
using Portal.Entities;

namespace Portal.Mappings
{
	public class EmployeePollItemMap : ClassMap<EmployeePollItem>
	{
        public EmployeePollItemMap()
        {
			Id(x => x.Id);

			Map(x => x.Option1Count);
			Map(x => x.Option2Count);
			Map(x => x.Option3Count);
			Map(x => x.Option4Count);
            Map(x => x.Option5Count);
            Map(x => x.Option6Count);
            Map(x => x.AnswerValue);
            Map(x => x.AnswerText);
            Map(x => x.SessionId);

            References(x => x.Employee);
            References(x => x.PollItem);
            References(x => x.PollTaking);
        }
	}
}