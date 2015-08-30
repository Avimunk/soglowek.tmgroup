using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class Wiki : Entity
    {
        public virtual string Title { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Letter { get; set; }
        public virtual string LetterEn { get; set; }
        public virtual string AddedBy{ get; set; }
        public virtual string Content { get; set; }
        
    }
}