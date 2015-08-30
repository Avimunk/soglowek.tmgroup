using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class Like :Entity
    {
        public virtual Employee Employees { get; set; }
        public virtual Forum Forums { get; set; }
    }
}