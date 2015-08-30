using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
    public class JobTitle : Entity
    {
        public virtual string Name { get; set; }
        public virtual IList<Employee> Employees { get; set; }

    }
}
