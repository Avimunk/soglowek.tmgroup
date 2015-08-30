using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class FlashBanner : Entity
    {
        public virtual bool IsActive { get; set; }
        public virtual string Name { get; set; }
        public virtual string YouTubeCode { get; set; }
        public virtual string Content { get; set; }
        public virtual string Content2 { get; set; }
        public virtual string Banner { get; set; }
        public virtual int BannerOrder { get; set; }
        public virtual string Url { get; set; }
        public virtual int TypeId { get; set; }
    }
}