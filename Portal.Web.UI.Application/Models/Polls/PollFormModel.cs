using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Portal.Entities;
using FluentValidation;
using FluentValidation.Attributes;
using NHibernate;
using NHibernate.Linq;

namespace Portal.Models.Polls
{
    public class PollFormModel
    {
        public void InitMembers(ISession session)
        {
            this.Items = session.QueryOver<PollItem>()
                .Where(x => x.Poll.Id == this.Id)
                .List();
        }
        public long Id { get; set; }

        [Required(ErrorMessage = "השם הינו שדה חובה")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public bool Anonymous { get; set; }
        public bool Enabled { get; set; }
        public string DisabledContent { get; set; }
        public bool UserEditable { get; set; }
        public IList<PollItem> Items { get; set; }
    }
}