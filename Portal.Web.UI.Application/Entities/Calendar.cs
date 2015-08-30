using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Entities {
    public class Calendar {
        public virtual long Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Body { get; set; }
        public virtual string Abstract { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Url { get; set; }
        public virtual string Hours { get; set; }
        public virtual string Minutes { get; set; }
        public virtual string HoursUntill { get; set; }
        public virtual string MinutesUntill { get; set; }
        public virtual string Place { get; set; }
        public virtual string Teacher { get; set; }
        public virtual string Audience { get; set; }
        
    }
}