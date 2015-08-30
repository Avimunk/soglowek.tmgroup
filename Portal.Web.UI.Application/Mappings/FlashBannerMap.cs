using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class FlashBannerMap : ClassMap<FlashBanner>
    {
        public FlashBannerMap()
        {
            Id(x => x.Id);

            Map(x => x.IsActive);
            Map(x => x.Banner);
            Map(x => x.Content).Length(4000);
            Map(x => x.Content2).Length(4000);
            Map(x => x.Name);
            Map(x => x.Url);
            Map(x => x.TypeId);
            Map(x => x.YouTubeCode);
            Map(x => x.BannerOrder);
        }
    }
}