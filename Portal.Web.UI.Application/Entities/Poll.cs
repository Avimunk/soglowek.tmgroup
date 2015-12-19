using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;
using Portal.Helpers;

namespace Portal.Entities
{
	public class Poll : Entity
	{
        public Poll()
        {
            CreatedDate = DateTime.Now;
        }

        public virtual Poll Object { get; set; }
		public virtual DateTime CreatedDate { get; set; }
		public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Summary { get; set; }
        public virtual bool Anonymous { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual string DisabledContent { get; set; }
        public virtual bool UserEditable { get; set; }
        public virtual bool ReadOnly { get; set; }
        public virtual bool LayoutDirection { get; set; }
        public virtual bool RequireDepartment { get; set; }
        public virtual bool RequireManager { get; set; }
        public virtual string WindowTarget { get; set; }
        public virtual Employee AnonymousUser { get; set; }
        public virtual string StartCaption { get; set; }
        public virtual string EndCaption { get; set; }
        public virtual string IrrelevantCaption { get; set; }
        public virtual int LCID { get; set; }

        public virtual string CultureDisplayName
        {
            get
            {
                return CultureHelper.GetNativeName(this.LCID);
            }
        }

        public virtual IList<PollItem> PollItems { get; set; }

      
	}
}