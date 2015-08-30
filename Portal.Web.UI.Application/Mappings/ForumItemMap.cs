using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class ForumItemMap : ClassMap<ForumItem>
    {
        public ForumItemMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            References(x => x.Admin);
            Map(x => x.Active);
            Map(x => x.ForumUsers).Length(99999999);
            Map(x => x.isAvailableTooAll);
        }
    }
}