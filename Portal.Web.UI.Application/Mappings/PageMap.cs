using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class PageMap : ClassMap<Page>
	{
		public PageMap() {
			Id(x => x.Id);

			Map(x => x.IsActive);
			Map(x => x.IsBlank);
			Map(x => x.IsIFrame);
			Map(x => x.Name);
            Map(x => x.Content).Length(9999999);
			Map(x => x.Url);
			Map(x => x.SectionId);
			Map(x => x.TypeId);
			Map(x => x.PageOrder);
            
			Map(x => x.Department);
			Map(x => x.JobTitle);


            Map(x => x.PageTemplate);
            Map(x => x.Iframelink1);
            Map(x => x.Iframelink2);
            Map(x => x.Iframelink3);
            Map(x => x.Iframelink4);
            Map(x => x.Iframelink5);



            Map(x => x.Iframelink1DelayTime);
            Map(x => x.Iframelink2DelayTime);
            Map(x => x.Iframelink3DelayTime);
            Map(x => x.Iframelink4DelayTime);
            Map(x => x.Iframelink5DelayTime);

            Map(x => x.Mp4Content);
            


			
			Map(x => x.Employee);
			Map(x => x.IsInternal);
			Map(x => x.UpdatedDate);
            Map(x => x.Picture);
			References(x => x.Parent).ForeignKey("Parent_Id");


			HasMany(x => x.Children).OrderBy("PageOrder").KeyColumn("Parent_Id").Cascade.AllDeleteOrphan();
		}
	}
}