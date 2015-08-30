using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class WikiMap : ClassMap<Wiki>
    {
        public WikiMap()
        {
            Id(x => x.Id);
            Map(x => x.Title);
            Map(x => x.AddedBy);
            Map(x => x.Date);
            Map(x => x.Letter);
            Map(x => x.LetterEn);
            Map(x => x.Content).Length(9999999);
        }
    }
}