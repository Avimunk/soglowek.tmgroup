using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Data;

namespace Portal.Entities
{
	public class Setting : Entity
	{
        public virtual string FirstHpBody { get; set; }
        public virtual string SecondHpBody { get; set; }
        public virtual string GalleryBody { get; set; }
        public virtual string Manager { get; set; }
        public virtual string ManagerLink { get; set; }

        public virtual string Box1 { get; set; }
        public virtual string Box2 { get; set; }
         public virtual string Box3 { get; set; }

         public virtual string Box1Link { get; set; }
         public virtual string Box2Link { get; set; }
         public virtual string Box3Link { get; set; }

         public virtual string BoxRight { get; set; }

        // 4 inner boxes
         public virtual string hrtxt { get; set; }
         public virtual string revahatxt { get; set; }
         public virtual string salestxt { get; set; }
         public virtual string moretxt { get; set; }
        //
         public virtual string managerTableTxt { get; set; }
         public virtual string usefullLinksTxt { get; set; }
         public virtual string innerSysyemsTxt { get; set; }
        //
         public virtual string mainUpperTxt { get; set; }
        //
         public virtual string youtubetxt { get; set; }
         public virtual string youtubelink { get; set; }
         public virtual string youtubeContent { get; set; }
        //
         public virtual string Forumlink { get; set; }
        //
         public virtual string KeilaTxt { get; set; }
         public virtual string KeilaLink { get; set; }

         public virtual bool ActiveForderUpload { get; set; }

         public virtual string BDayContent { get; set; }

         public virtual string BDayEmails { get; set; }
         public virtual bool BDayEmailsAllow { get; set; }

         public virtual IList<Department> Departments { get; set; }

	}
}