using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Entities
{
    public class Log
    {
        public virtual Guid id { get; set; }
        public virtual int UpdatedCount { get; set; }
        public virtual int ErrorUpdatedCount { get; set; }
        public virtual DateTime Logdate { get; set; }
        public virtual string ErrorLines { get; set; }
        public virtual string NewEmps { get; set; }
        public virtual string WarningLines { get; set; }
        public virtual string Added { get; set; }
        public virtual string Deleted { get; set; }


    }
}