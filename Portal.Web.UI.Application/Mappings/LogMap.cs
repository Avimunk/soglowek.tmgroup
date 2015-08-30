using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class LogMap : ClassMap<Log>
    {
        public LogMap()
        {
            Id(x => x.id);
            Map(x => x.ErrorLines).CustomType("StringClob");
            Map(x => x.Logdate);
            Map(x => x.UpdatedCount);
            Map(x => x.NewEmps);
            Map(x => x.WarningLines);

            Map(x => x.Added);
            Map(x => x.Deleted);
     

 
        }
    }
}