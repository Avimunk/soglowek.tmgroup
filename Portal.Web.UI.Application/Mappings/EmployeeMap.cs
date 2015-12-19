using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Portal.Entities;
using FluentNHibernate.Mapping;

namespace Portal.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap() {
            Id(x => x.Id).GeneratedBy.Assigned();

            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.EnglishFirstName);
            Map(x => x.EnglishLastName);
            Map(x => x.Alias);
            Map(x => x.Username);
            Map(x => x.Phone);
            Map(x => x.Mobile);
            Map(x => x.Range);
            Map(x => x.Email);
            Map(x => x.BDay);
            Map(x => x.IsActive);
            Map(x => x.Ip);
            Map(x => x.PostContent);
            Map(x => x.IsAdmin);
            Map(x => x.EmployeeMessagesCount).Formula("(select count(*) from " + MvcApplication.Config("table.Prefix") + "EmployeeMessage where " + MvcApplication.Config("table.Prefix") + "EmployeeMessage.To_Id=Id and " + MvcApplication.Config("table.Prefix") + "EmployeeMessage.IsRead='false')"); ;
            Map(x => x.StartYear);
            Map(x => x.Picture);
            Map(x => x.Local_id);
         
          
            References(x => x.Department);
            References(x => x.JbTitle);

            HasMany(x => x.EmployeePhotos);
            HasMany(x => x.EmployeeMessages).KeyColumn("To_id");
            HasMany(x => x.SentEmployeeMessages).KeyColumn("CreatedBy_id");
            HasMany(x => x.Likes);
            References(x => x.Manager).ForeignKey("Manager_id");

            Map(x => x.Password);
        }
    }
}