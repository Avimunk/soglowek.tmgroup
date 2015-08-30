using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;
using System.IO;

namespace Portal.Entities {
    public class Gallery : Entity {
        public virtual new Guid Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Title { get; set; }
        public virtual string DefaultPhoto { get; set; }
       

        public virtual IList<string> GetNames {
            get {
                var path = HttpContext.Current.Server.MapPath("~/public/userfiles/galleries/small/" + Id);
                return Directory.GetFiles(path).Select(x => Path.GetFileName(x)).ToList();
            }
        }
    }
}