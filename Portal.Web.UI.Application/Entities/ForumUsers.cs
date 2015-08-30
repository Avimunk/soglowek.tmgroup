using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class ForumUsers : Entity
	{

        public virtual Employee User { get; set; }
        public virtual ForumItem Forum { get; set; }


	}
}