using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Models.Calendars;
using Portal.Entities;
using AutoMapper;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Criterion;

namespace Portal.Controllers
{
    public class CalendarsController : ApplicationController
    {
        [Authorize]
        public ActionResult Index() {
            var items = GetSession.QueryOver<Calendar>()
                .OrderBy(x => x.Date).Asc
                .List();
            return View(items);
        }

        [Authorize]
        public ActionResult Create() {
            var model = new CalendarFormModel() {
                Date = DateTime.Now
            };
            return View(model);
        }


        [Authorize, HttpPost, Transaction]
        public ActionResult Create(CalendarFormModel model) {

            var item = Mapper.Map<CalendarFormModel, Calendar>(model);
            GetSession.Save(item);

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Edit(long id) {
            var item = GetSession.Get<Calendar>(id);

            var model = Mapper.Map<Calendar, CalendarFormModel>(item);

            return View(model);
        }

        [HttpPost, Authorize, Transaction]
        public ActionResult Edit(CalendarFormModel model) {
            var item = GetSession.Get<Calendar>(model.Id);

            Mapper.Map<CalendarFormModel, Calendar>(model, item);
            GetSession.Update(item);

            return RedirectToAction("Index");
        }


        [Authorize, Transaction]
        public ActionResult Destroy(long id) {
            var item = GetSession.Get<Calendar>(id);
            GetSession.Delete(item);
            return RedirectToAction("Index");
        }

        public ActionResult List(string date) {
            DateTime selectedDate;

            if (!DateTime.TryParse(date, out selectedDate)) {
                selectedDate = DateTime.Now;
            }

            ViewBag.SelectedDate = selectedDate;

            //var items = GetSession.CreateQuery("from Calendar where Month(Date)=:month and Year(Date)=:year Order by date")
            //    .SetInt32("month", month)
            //    .SetInt32("year", year)
            //    .List<Calendar>();
            

            var items = GetSession.QueryOver<Calendar>()
                .Where(x => x.Date == selectedDate)
                .OrderBy(x => x.Title).Asc
                .List();

            return View(items);
        }





        public ActionResult Show(long id)
        {


            var item = GetSession.Get<Calendar>(id);

            return View(item);
        }




















        [ChildActionOnly]
        public ActionResult CalendarsBox() {

            return PartialView();
        }


        public ActionResult GetAllTasks(string date)
        {
            DateTime selectedDate;

            if (!DateTime.TryParse(date, out selectedDate))
            {
                selectedDate = DateTime.Now;
            }

            ViewBag.SelectedDate = selectedDate;

            //var items = GetSession.CreateQuery("from Calendar where Month(Date)=:month and Year(Date)=:year Order by date")
            //    .SetInt32("month", month)
            //    .SetInt32("year", year)
            //    .List<Calendar>();

            var items = GetSession.QueryOver<Calendar>()
                .Where(x => x.Date == selectedDate)
                .OrderBy(x => x.Title).Asc
                .List();




            return PartialView(items);
        }



        [HttpPost]
        public JsonResult GetDates() {
            var items = GetSession.QueryOver<Calendar>()
                .List()
                .Select(x => new {
                    month = x.Date.Month,
                    day = x.Date.Day,
                    title = x.Title
                });
            return Json(items);
        }


        public JsonResult GetPerson(string name)
        {
            var items = GetSession.QueryOver<Employee>()
                .Where(x => x.FirstName.IsLike(name, MatchMode.Anywhere))
                .List()
                .Select(x => new
                {
                    Name = x.FirstName,
                    Phone = x.Phone
                });
            return Json(items,JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetNews(string date)
        {

            DateTime selectedDate;

            if (!DateTime.TryParse(date, out selectedDate))
            {
                selectedDate = DateTime.Now;
            }


            var items = GetSession.QueryOver<Calendar>().Where(x => x.Date == selectedDate)
                .List()
                .Select(x => new
                {
                    Hour = x.Hours,
                    Minute = x.Minutes,
                    Title = x.Title,
                    Abstract = x.Abstract,
                    Place = x.Place

                });
            return Json(items);
        }



    }
}
