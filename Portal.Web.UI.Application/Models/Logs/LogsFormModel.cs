using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace Portal.Models.logs
{
    public class logsFormModel
    {
        public virtual Guid id { get; set; }
        public virtual int UpdatedCount { get; set; }
        public virtual int ErrorUpdatedCount { get; set; }
        public virtual DateTime Logdate { get; set; }
        public virtual string ErrorLines { get; set; }  
    }
}