using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings {
    public class CalendarMap : ClassMap<Calendar> {
        public CalendarMap() {
            Id(x => x.Id);

            Map(x => x.Title);
            Map(x => x.Date);
            Map(x => x.Url);
            Map(x => x.Abstract).CustomType("StringClob");
            Map(x => x.Body).CustomType("StringClob");
            Map(x => x.Hours);
            Map(x => x.Minutes);
            Map(x => x.HoursUntill);
            Map(x => x.MinutesUntill);
            Map(x => x.Audience);
            Map(x => x.Teacher);
            Map(x => x.Place);
        }
    }
}