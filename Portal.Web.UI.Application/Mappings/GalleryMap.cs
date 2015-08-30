using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;
 
namespace Portal.Mappings
{
    public class GalleryMap : ClassMap<Gallery>
    {
        public GalleryMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.Title);
            Map(x => x.Date);

            Map(x => x.DefaultPhoto);
        }
    }
}