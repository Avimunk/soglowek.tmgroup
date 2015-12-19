using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Portal.Entities;
using FluentValidation;
using FluentValidation.Attributes;
using NHibernate;
using NHibernate.Linq;
using System.Globalization;

namespace Portal.Models.Polls
{
    public class PollFormModel
    {
        public PollFormModel()
        {
            // Cultures
            this.Cultures = new List<SelectListItem>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures).OrderBy(c => c.DisplayName))
            {
                this.Cultures.Add(new SelectListItem()
                    {
                        Value = culture.LCID.ToString(),
                        Text = culture.DisplayName
                    });
            }
        }


        public void InitMembers(ISession session)
        {
            this.Items = session.QueryOver<PollItem>()
                .Where(x => x.Poll.Id == this.Id)
                .List();


            // Objects
            this.Objects = session.QueryOver<Poll>()
                .Where(p => p.Id != this.Id)
                .List();

            // Anonymous Users
            IList<Employee> users = session.QueryOver<Employee>()
                .List();
            if (this.AnonymousUser_id.GetValueOrDefault() > 0)
            {
                foreach (Employee user in users)
                {
                    if (user.Id == 0)
                    {
                        user.Id = this.AnonymousUser_id.GetValueOrDefault();
                    }
                }
            }
            this.AnonymousUsers = users;

        }
        public long Id { get; set; }
        public long? Object_id { get; set; }
        public long? AnonymousUser_id { get; set; }
        

        [Required(ErrorMessage = "השם הינו שדה חובה")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public bool Anonymous { get; set; }
        public bool Enabled { get; set; }
        public string DisabledContent { get; set; }
        public bool UserEditable { get; set; }
        public bool RequireDepartment { get; set; }
        public bool RequireManager { get; set; }
        public string WindowTarget { get; set; }
        public string StartCaption { get; set; }
        public string EndCaption { get; set; }
        public string IrrelevantCaption { get; set; }
        public int LCID { get; set; }

        public IList<SelectListItem> Cultures { get; set; }
        
        public IList<Poll> Objects { get; set; }
        public IList<Employee> AnonymousUsers { get; set; }

        public IList<PollItem> Items { get; set; }
    }
}