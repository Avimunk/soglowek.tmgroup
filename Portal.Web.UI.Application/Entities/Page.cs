using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class Page : Entity
	{
		public virtual DateTime? UpdatedDate { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool IsIFrame{ get; set; }
		public virtual bool IsBlank { get; set; }
		public virtual string Name { get; set; }
		public virtual string Content { get; set; }
		public virtual string Url { get; set; }
	
		
		public virtual string JobTitle { get; set; }
		public virtual string Employee { get; set; }
		public virtual string Department { get; set; }
		public virtual IList<Page> Children { get; set; }
		public virtual Page Parent { get; set; }
		public virtual int SectionId { get; set; }
		public virtual int TypeId { get; set; }
        public virtual string PageTemplate { get; set; }
        public virtual int PageOrder { get; set; }
		public virtual bool IsInternal { get; set; }
        public virtual string Picture { get; set; }
        public virtual string Iframelink1 { get; set; }
        public virtual string Iframelink2 { get; set; }
        public virtual string Iframelink3 { get; set; }
        public virtual string Iframelink4 { get; set; }
        public virtual string Iframelink5 { get; set; }

        public virtual string Iframelink1DelayTime { get; set; }
        public virtual string Iframelink2DelayTime { get; set; }
        public virtual string Iframelink3DelayTime { get; set; }
        public virtual string Iframelink4DelayTime { get; set; }
        public virtual string Iframelink5DelayTime { get; set; }

        public virtual string Mp4Content { get; set; }

        

	}
}