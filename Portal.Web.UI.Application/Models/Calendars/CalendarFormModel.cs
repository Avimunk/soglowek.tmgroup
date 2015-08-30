using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Models.Calendars {
    public class CalendarFormModel {

        public CalendarFormModel()
        {
            AllHours = new List<SelectListItem>();
            AllHoursUntill = new List<SelectListItem>();
            
            for (int i = 0; i < 24; i++)
			{
                if (i < 10)
                {
                    AllHours.Add(new SelectListItem()
                    {
                        Text='0'+i.ToString(),
                        Value='0'+i.ToString()
                    });

                    AllHoursUntill.Add(new SelectListItem()
                    {
                        Text = '0' + i.ToString(),
                        Value = '0' + i.ToString()
                    });
                }
                else
                {
                    AllHours.Add(new SelectListItem()
                    {
                        Text =i.ToString(),
                        Value =i.ToString()
                    });

                    AllHoursUntill.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
			}

            AllMinutes = new List<SelectListItem>();
            AllMinutesUntill = new List<SelectListItem>();
            for (int i = 0; i < 60; i++)
            {
                if (i < 10)
                {
                    AllMinutes.Add(new SelectListItem()
                    {
                        Text = '0' + i.ToString(),
                        Value = '0' + i.ToString()
                    });
                    AllMinutesUntill.Add(new SelectListItem()
                    {
                        Text = '0' + i.ToString(),
                        Value = '0' + i.ToString()
                    });
                }
                else
                {
                    AllMinutes.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });

                    AllMinutesUntill.Add(new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }
            }
         
        }
        public long Id { get; set; }
        public string Abstract { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }

        public IList<SelectListItem> AllHours { get; set; }
        public IList<SelectListItem> AllMinutes { get; set; }
        public string Hours { get; set; }
        public string Minutes { get; set; }

        public IList<SelectListItem> AllHoursUntill { get; set; }
        public IList<SelectListItem> AllMinutesUntill { get; set; }
        public virtual string HoursUntill { get; set; }
        public virtual string MinutesUntill { get; set; }
        public virtual string Place { get; set; }
        public virtual string Teacher { get; set; }
        public virtual string Audience { get; set; }


    }
}