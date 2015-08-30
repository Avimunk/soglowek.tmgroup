using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class LikeMap : ClassMap<Like>
    {
        public LikeMap()
        {
            Id(x => x.Id);
            References(x => x.Employees);
            References(x => x.Forums);
        }
    }
}