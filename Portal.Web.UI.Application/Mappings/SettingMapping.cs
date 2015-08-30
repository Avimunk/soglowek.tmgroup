using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
	public class SettingMapping : ClassMap<Setting>
	{
		public SettingMapping() {
			Id(x => x.Id);

            Map(x => x.FirstHpBody).CustomType("StringClob");
            Map(x => x.SecondHpBody).CustomType("StringClob");
            Map(x => x.GalleryBody).CustomType("StringClob");
            Map(x => x.BoxRight).CustomType("StringClob");
            Map(x => x.Manager).CustomType("StringClob");
            Map(x => x.ManagerLink);
            Map(x => x.Box1);
            Map(x => x.Box2);
            Map(x => x.Box3);

            Map(x => x.Box1Link);
            Map(x => x.Box2Link);
            Map(x => x.Box3Link);



            Map(x => x.hrtxt).Length(9999999);
            Map(x => x.revahatxt).Length(9999999);
            Map(x => x.salestxt).Length(9999999);
            Map(x => x.moretxt).Length(9999999);

            //

            Map(x => x.managerTableTxt).Length(9999999);
            Map(x => x.usefullLinksTxt).Length(9999999);
            Map(x => x.innerSysyemsTxt).Length(9999999);
            //
            Map(x => x.mainUpperTxt).Length(9999999);

            Map(x => x.youtubetxt).Length(9999999);
            Map(x => x.youtubelink).Length(9999999);

            Map(x => x.youtubeContent).Length(9999999);
            Map(x => x.Forumlink).Length(9999999);

            //
            Map(x => x.KeilaTxt);
            Map(x => x.KeilaLink);

            Map(x => x.ActiveForderUpload);

            Map(x => x.BDayContent).Length(9999999);


            Map(x => x.BDayEmails).Length(9999999);
            Map(x => x.BDayEmailsAllow);
            
		}
	}
}