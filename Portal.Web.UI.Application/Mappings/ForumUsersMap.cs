using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class ForumUsersMap : ClassMap<ForumUsers>
    {
        public ForumUsersMap()
        {
            Id(x => x.Id);
            References(x => x.User);
            References(x => x.Forum);

        }
    }
}