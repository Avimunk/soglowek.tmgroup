using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using Portal.Models.Polls;
using Portal.Entities;
using Portal.Attributes;
using Portal.Helpers;
using AutoMapper;
using NHibernate.Linq;
using System.Text.RegularExpressions;

namespace Portal.Controllers
{
    [Employee]
    [SessionState(System.Web.SessionState.SessionStateBehavior.Required)]
    public class PollsController : ApplicationController
    {
        protected long? CurrentPollId
        {
            get
            {
                if (Session["Poll_Id"] != null)
                {
                    return Convert.ToInt64(Session["Poll_Id"]);
                }

                return null;
            }
            set
            {
                Session["Poll_Id"] = value;
            }
        }
        protected long? CurrentPollDepartmentId
        {
            get
            {
                if (Session["Poll_Department_Id"] != null)
                {
                    return Convert.ToInt64(Session["Poll_Department_Id"]);
                }

                return null;
            }
            set
            {
                Session["Poll_Department_Id"] = value;
            }
        }
        protected long? CurrentPollManagerId
        {
            get
            {
                if (Session["Poll_Manager_Id"] != null)
                {
                    return Convert.ToInt64(Session["Poll_Manager_Id"]);
                }

                return null;
            }
            set
            {
                Session["Poll_Manager_Id"] = value;
            }
        }

        protected int? CurrentPollLCID
        {
            get
            {
                if (Session["Poll_LCID"] != null)
                {
                    return Convert.ToInt32(Session["Poll_LCID"]);
                }

                return null;
            }
            set
            {
                Session["Poll_LCID"] = value;
            }
        }



        [Admin]
        public ActionResult Index()
        {
            var items = GetSession.QueryOver<Poll>()
                .List();
            return View(items);
        }

        [Admin]
        public ActionResult Create()
        {
            var model = new PollFormModel();

            model.InitMembers(GetSession);

            return View(model);
        }

        [Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Create(PollFormModel model)
        {
            if (ModelState.IsValid)
            {
                var item = Mapper.Map<PollFormModel, Poll>(model);


                // Object
                if (model.Object_id > 0)
                {
                    item.Object = GetSession.Get<Poll>(model.Object_id);
                }
                else
                {
                    item.Object = null;
                }

                // Anonymous User
                if (model.AnonymousUser_id > 0)
                {
                    item.AnonymousUser = GetSession.Get<Employee>(model.AnonymousUser_id);
                }
                else
                {
                    item.AnonymousUser = null;
                }


                GetSession.Save(item);

                return RedirectToAction("edit", new { id = item.Id });
            }

            return View(model);
        }

        [Admin]
        public ActionResult Edit(long id)
        {

            var item = GetSession.Get<Poll>(id);

            var model = Mapper.Map<Poll, PollFormModel>(item);

            model.InitMembers(GetSession);

            return View(model);
        }

        [Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Edit(PollFormModel model)
        {
            var item = GetSession.Get<Poll>(model.Id);

            //bool anonymous = model.Anonymous;
            //if (!model.Anonymous && item.Anonymous)
            //{
            //    if (item.PollItems.Any(pi => pi.EmployeePollItems.Count > 0))
            //    {
            //        anonymous = true;
            //    }
            //}

            if (ModelState.IsValid)
            {
                Mapper.Map<PollFormModel, Poll>(model, item);

                item.Anonymous = model.Anonymous;
                // Object
                if (model.Object_id > 0)
                {
                    item.Object = GetSession.Get<Poll>(model.Object_id);
                }
                else
                {
                    item.Object = null;
                }

                // Anonymous User
                if (model.AnonymousUser_id > 0)
                {
                    item.AnonymousUser = GetSession.Get<Employee>(model.AnonymousUser_id);
                }
                else
                {
                    item.AnonymousUser = null;
                }

                GetSession.Update(item);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Admin, Transaction]
        public ActionResult Destroy(long id)
        {

            var item = GetSession.Get<Poll>(id);

            GetSession.Delete(item);

            return RedirectToAction("Index");
        }

        public ActionResult Archive()
        {
            var items = GetSession.QueryOver<Poll>()
                .OrderBy(x => x.CreatedDate)
                .Desc
                .Skip(1)
                .List();
            return View(items);
        }

        public ActionResult Show(Int64 id)
        {
            //if (Session["optionResult" + DateTime.Now.Date.ToShortDateString()] !=null)
            // {

            Poll item = GetSession.Get<Poll>(id);

            bool answered = item.PollItems.Any(pi => pi.EmployeePollItems.Any(epi => epi.Employee.Id == Portal.Helpers.EmployeesHelper.GetCurrentEmployee().Id));

            if (answered && !item.UserEditable)
            {
                item.ReadOnly = true;
            }
            //var items = GetSession.QueryOver<Poll>()
            //    .Where(x => x.Id == id)
            //        .List();
            return View(item);
            // }
            // else
            // {

            //    return RedirectToAction("PollResult");
            // }
        }


        [HttpPost, Transaction]
        public ActionResult Show(Int64 id, List<int> optionResult)
        {
            Poll poll = GetSession.Get<Poll>(id);
            for (int i = 0; i < optionResult.Count; i++)
            {
                PollItem item = poll.PollItems[i];

                //session.CreateSQLQuery(string.Format("Update {0}PollItem set Option{1}Count=Option{1}Count+1 where Id={2}",
                //  MvcApplication.Config("table.Prefix"),
                //  optionResult[i],
                //  item.Id)).ExecuteUpdate();

                Session["optionResult" + DateTime.Now.Date.ToShortDateString()] = "done";

                try
                {
                    EmployeePollItem employeePollItem = GetSession.QueryOver<EmployeePollItem>()
                        .Where(epi => epi.Employee.Id == Portal.Helpers.EmployeesHelper.GetCurrentEmployee().Id &&
                            epi.PollItem.Id == item.Id)
                        .SingleOrDefault();

                    string query = String.Empty;

                    if (employeePollItem == null)
                    {
                        employeePollItem = new EmployeePollItem()
                        {
                            Employee = Portal.Helpers.EmployeesHelper.GetCurrentEmployee(),
                            PollItem = item
                        };
                    }
                    switch (optionResult[i])
                    {
                        case 1:
                            employeePollItem.Option1Count = 1;
                            employeePollItem.Option2Count = 0;
                            employeePollItem.Option3Count = 0;
                            employeePollItem.Option4Count = 0;
                            employeePollItem.Option5Count = 0;
                            employeePollItem.Option6Count = 0;
                            //query = string.Format("Update {0}EmployeePollItem set Option1Count=1,Option2Count=0,Option3Count=0,Option4Count=0,Option5Count=0,Option6Count=0 where Id={1}",
                            //            MvcApplication.Config("table.Prefix"),
                            //            employeePollItem.Id);
                            break;
                        case 2:
                            employeePollItem.Option1Count = 0;
                            employeePollItem.Option2Count = 1;
                            employeePollItem.Option3Count = 0;
                            employeePollItem.Option4Count = 0;
                            employeePollItem.Option5Count = 0;
                            employeePollItem.Option6Count = 0;
                            break;
                        case 3:
                            employeePollItem.Option1Count = 0;
                            employeePollItem.Option2Count = 0;
                            employeePollItem.Option3Count = 1;
                            employeePollItem.Option4Count = 0;
                            employeePollItem.Option5Count = 0;
                            employeePollItem.Option6Count = 0;
                            break;
                        case 4:
                            employeePollItem.Option1Count = 0;
                            employeePollItem.Option2Count = 0;
                            employeePollItem.Option3Count = 0;
                            employeePollItem.Option4Count = 1;
                            employeePollItem.Option5Count = 0;
                            employeePollItem.Option6Count = 0;
                            break;
                        case 5:
                            employeePollItem.Option1Count = 0;
                            employeePollItem.Option2Count = 0;
                            employeePollItem.Option3Count = 0;
                            employeePollItem.Option4Count = 0;
                            employeePollItem.Option5Count = 1;
                            employeePollItem.Option6Count = 0;
                            break;
                        case 6:
                            employeePollItem.Option1Count = 0;
                            employeePollItem.Option2Count = 0;
                            employeePollItem.Option3Count = 0;
                            employeePollItem.Option4Count = 0;
                            employeePollItem.Option5Count = 0;
                            employeePollItem.Option6Count = 1;
                            break;
                    }

                    GetSession.Save(employeePollItem);

                    //query = string.Format("INSERT INTO [{0}EmployeePollItem] ([Employee_id],[PollItem_id],[Option{1}Count]) VALUES ({2}, {3}, {4})",
                    //  MvcApplication.Config("table.Prefix"),
                    //  optionResult[i],
                    //  Portal.Helpers.EmployeesHelper.GetCurrentEmployee().Id,
                    //  item.Id,
                    //  1);

                    //GetSession.CreateSQLQuery(query).ExecuteUpdate();
                }
                catch { }
            }

            return RedirectToAction("PollResult", new { id = id });
        }


        public ActionResult Take(Int64 id)
        {
            Poll item = GetSession.Get<Poll>(id);

            if (item != null)
            {
                bool answered = item.PollItems.Any(pi => pi.EmployeePollItems.Any(epi => epi.Employee.Id == Portal.Helpers.EmployeesHelper.GetCurrentEmployee().Id));

                if (answered && !item.UserEditable && !(item.AnonymousUser != null && item.AnonymousUser.Id == EmployeesHelper.GetCurrentEmployee().Id))
                {
                    item.ReadOnly = true;
                }

                if (this.CurrentPollId == id)
                {
                    return View(new PollTakeFormModel()
                        {
                            Poll = item,
                            IsRightToLeft = CurrentPollLCID.HasValue ? CultureHelper.IsRightToLeft(CurrentPollLCID.Value) : true
                        });
                }
                else
                {
                    return RedirectToAction("TakeOptions", new { Id = id });
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Take(Int64 id, PollTakeFormModel model)
        {
            Poll poll = GetSession.Get<Poll>(id);

            if (poll != null && poll.Id == CurrentPollId)
            {
                try
                {
                    Guid sessionId = Guid.NewGuid();

                    PollTaking pollTaking = new PollTaking()
                    {
                        Poll = poll,
                        Employee = Portal.Helpers.EmployeesHelper.GetCurrentEmployee(),
                        Department_id = CurrentPollDepartmentId.HasValue ? CurrentPollDepartmentId.Value : 0,
                        Manager = CurrentPollManagerId.HasValue ? GetSession.Get<Employee>(CurrentPollManagerId.Value) : null,
                        SessionId = sessionId
                    };
                    GetSession.Save(pollTaking);

                    Regex regex = new Regex(@"\[[0-9]+\]", RegexOptions.IgnoreCase);

                    foreach (string key in Request.Form.AllKeys)
                    {
                        MatchCollection matches = regex.Matches(key);

                        if (matches.Count > 0)
                        {
                            char[] charsToTrim = { '[', ']' };

                            long itemId = Convert.ToInt64(matches[0].Value.Trim(charsToTrim)); ;
                            string value = this.Request.Form[key];

                            if (!String.IsNullOrWhiteSpace(value))
                            {
                                PollItem item = GetSession.Get<PollItem>(itemId);

                                EmployeePollItem employeePollItem = GetSession.QueryOver<EmployeePollItem>()
                        .Where(epi => epi.Employee.Id == Portal.Helpers.EmployeesHelper.GetCurrentEmployee().Id &&
                            epi.PollItem.Id == item.Id)
                        .Take(1)
                        .SingleOrDefault();

                                if (employeePollItem == null || (poll.AnonymousUser != null && poll.AnonymousUser.Id == EmployeesHelper.GetCurrentEmployee().Id))
                                {
                                    employeePollItem = GetSession.QueryOver<EmployeePollItem>()
                        .Where(epi => epi.Employee.Id == Portal.Helpers.EmployeesHelper.GetCurrentEmployee().Id &&
                            epi.PollItem.Id == item.Id && epi.SessionId == sessionId)
                        .Take(1)
                        .SingleOrDefault();

                                    if (employeePollItem == null)
                                    {
                                        employeePollItem = new EmployeePollItem()
                                        {
                                            Employee = Portal.Helpers.EmployeesHelper.GetCurrentEmployee(),
                                            PollItem = item
                                        };
                                    }
                                }

                                employeePollItem.PollTaking = pollTaking;

                                // Irrelevant question
                                if (key.ToLower().Contains("irrelevant"))
                                {
                                    if (value.Equals("on", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        employeePollItem.AnswerValue = -1;
                                    }
                                }
                                else
                                {
                                    switch (item.Type)
                                    {
                                        case QuestionType.Open:
                                            employeePollItem.AnswerText = value;
                                            break;
                                        case QuestionType.Range:
                                            employeePollItem.AnswerValue = Convert.ToInt32(Convert.ToDecimal(value));
                                            break;
                                        case QuestionType.Multiple:
                                            employeePollItem.AnswerValue = Convert.ToInt32(Convert.ToDecimal(value));
                                            switch (employeePollItem.AnswerValue)
                                            {
                                                case 1:
                                                    employeePollItem.Option1Count = 1;
                                                    employeePollItem.Option2Count = 0;
                                                    employeePollItem.Option3Count = 0;
                                                    employeePollItem.Option4Count = 0;
                                                    employeePollItem.Option5Count = 0;
                                                    employeePollItem.Option6Count = 0;
                                                    break;
                                                case 2:
                                                    employeePollItem.Option1Count = 0;
                                                    employeePollItem.Option2Count = 1;
                                                    employeePollItem.Option3Count = 0;
                                                    employeePollItem.Option4Count = 0;
                                                    employeePollItem.Option5Count = 0;
                                                    employeePollItem.Option6Count = 0;
                                                    break;
                                                case 3:
                                                    employeePollItem.Option1Count = 0;
                                                    employeePollItem.Option2Count = 0;
                                                    employeePollItem.Option3Count = 1;
                                                    employeePollItem.Option4Count = 0;
                                                    employeePollItem.Option5Count = 0;
                                                    employeePollItem.Option6Count = 0;
                                                    break;
                                                case 4:
                                                    employeePollItem.Option1Count = 0;
                                                    employeePollItem.Option2Count = 0;
                                                    employeePollItem.Option3Count = 0;
                                                    employeePollItem.Option4Count = 1;
                                                    employeePollItem.Option5Count = 0;
                                                    employeePollItem.Option6Count = 0;
                                                    break;
                                                case 5:
                                                    employeePollItem.Option1Count = 0;
                                                    employeePollItem.Option2Count = 0;
                                                    employeePollItem.Option3Count = 0;
                                                    employeePollItem.Option4Count = 0;
                                                    employeePollItem.Option5Count = 1;
                                                    employeePollItem.Option6Count = 0;
                                                    break;
                                                case 6:
                                                    employeePollItem.Option1Count = 0;
                                                    employeePollItem.Option2Count = 0;
                                                    employeePollItem.Option3Count = 0;
                                                    employeePollItem.Option4Count = 0;
                                                    employeePollItem.Option5Count = 0;
                                                    employeePollItem.Option6Count = 1;
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                employeePollItem.SessionId = sessionId;


                                GetSession.Save(employeePollItem);
                            }
                        }
                    }
                }
                catch
                {

                }
                
                Session["Poll_Id"] = null;
                Session["Poll_Department_Id"] = null;
                Session["Poll_Manager_Id"] = null;
                Session["Poll_LCID"] = null;

                return View("TakeEnd", new PollTakeEndFormModel()
                    {
                        Poll = poll,
                        IsRightToLeft = CurrentPollLCID.HasValue ? CultureHelper.IsRightToLeft(CurrentPollLCID.Value) : true
                    });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult TakeOptions(long id)
        {
            Poll item = GetSession.Get<Poll>(id);

            if (item != null)
            {
                if (item.AnonymousUser != null && item.AnonymousUser.Id == EmployeesHelper.GetCurrentEmployee().Id)
                {
                    PollTakeOptionsFormModel model = new PollTakeOptionsFormModel()
                    {
                        Poll = item,
                        IsRightToLeft = CurrentPollLCID.HasValue ? CultureHelper.IsRightToLeft(CurrentPollLCID.Value) : true
                    };

                    // Cultures
                    model.Cultures = new List<SelectListItem>();
                    IList<Poll> objects = GetSession.QueryOver<Poll>()
                        .Where(x => x.Id == id || (x.Object != null && x.Object.Id == id))
                        .List();
                    foreach (Poll pollObject in objects)
                    {
                        model.Cultures.Add(new SelectListItem()
                        {
                            Value = pollObject.LCID.ToString(),
                            Text = CultureHelper.GetNativeName(pollObject.LCID)
                        });
                    }

                    // Departments
                    model.Departments = new List<SelectListItem>();
                    IList<Department> departments = GetSession.QueryOver<Department>().List();
                    foreach (Department department in departments)
                    {
                        model.Departments.Add(new SelectListItem()
                        {
                            Value = department.Id.ToString(),
                            Text = department.Name
                        });
                    }

                    // Managers
                    model.Managers = new List<SelectListItem>();
                    IList<Employee> managers = GetSession.QueryOver<Employee>()
                        .Where(x => x.Manager == null)
                        .List();
                    foreach (Employee manager in managers)
                    {
                        model.Managers.Add(new SelectListItem()
                        {
                            Value = manager.Id.ToString(),
                            Text = manager.FullName
                        });
                    }

                    return View("TakeOptions", model);
                }
                else
                {
                    CurrentPollId = item.Id;
                    if (EmployeesHelper.GetCurrentEmployee().Department != null)
                    {
                        CurrentPollDepartmentId = EmployeesHelper.GetCurrentEmployee().Department.Id;
                    }
                    if (EmployeesHelper.GetCurrentEmployee().Manager != null)
                    {
                        CurrentPollManagerId = EmployeesHelper.GetCurrentEmployee().Manager.Id;
                    }
                    if (item.LCID > 0)
                    {
                        CurrentPollLCID = item.LCID;
                    }

                    return RedirectToAction("TakeIntro", new { Id = CurrentPollId });
                }
            }
            else
            {
                return RedirectToAction("Take", new { Id = id });
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TakeOptions(long Id, PollTakeOptionsFormModel model)
        {
            Poll item = GetSession.Get<Poll>(Id);

            if (item != null)
            {
                if (ModelState.IsValid)
                {
                    CurrentPollId = Id; 
                    
                    if (model.LCID > 0)
                    {
                        Poll poll = GetSession.QueryOver<Poll>()
                            .Where(x => x.Object != null && x.Object.Id == Id && x.LCID == model.LCID)
                            .SingleOrDefault();

                        if (poll != null)
                        {
                            CurrentPollId = poll.Id;
                        }
                    }

                    CurrentPollDepartmentId = model.Department_id;
                    CurrentPollManagerId = model.Manager_id;
                    CurrentPollLCID = model.LCID;
                }

                return RedirectToAction("TakeIntro", new { Id = CurrentPollId });
            }
            else
            {
                return RedirectToAction("Take", new { Id = Id });
            }
        }

        public ActionResult TakeIntro(long id)
        {
            Poll poll = GetSession.Get<Poll>(id);

            if (poll != null && poll.Id == CurrentPollId)
            {
                PollTakeIntroFormModel model = new PollTakeIntroFormModel()
                {
                    Poll = poll,
                    IsRightToLeft = CurrentPollLCID.HasValue ? CultureHelper.IsRightToLeft(CurrentPollLCID.Value) : true
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Take", new { Id = id });
            }
        }


        public ActionResult PollResult(Int64 id)
        {
            var item = GetSession.Get<Poll>(id);
            return View(item);
            //var items = GetSession.QueryOver<Poll>()
            //     .OrderBy(x => x.CreatedDate)
            //     .Desc
            //     .List();
            //return View(items);
        }

        public ActionResult ExportToExcel(long id, byte type = 1)
        {
            Poll poll = GetSession.QueryOver<Poll>()
                .Where(x => x.Id == id)
                .SingleOrDefault();

            var dataTable = new System.Data.DataTable("Poll");

            switch (type)
            {
                case 1:
                    dataTable.Columns.Add("QuestionNumber", typeof(int));
                    dataTable.Columns.Add("QuestionContent", typeof(string));
                    dataTable.Columns.Add("AnswerNumber", typeof(int));
                    dataTable.Columns.Add("AnswerCount", typeof(int));
                    dataTable.Columns.Add("AnswerPercentage", typeof(float));

                    List<PollItem> items = poll.PollItems.OrderBy(pi => pi.Id).ToList();

                    for (int i = 0; i < items.Count; i++)
                    {
                        PollItem item = items[i];

                        var results = item.EmployeePollItems.GroupBy(epi=>epi.AnswerValue,
                            (key, g) => new {
                                Value = key,
                                Count = g.Count()
                            }
                        );

                        int total = results.Sum(r => r.Count);

                        foreach(var result in results)
                        {
                            dataTable.Rows.Add(
                                (i + 1).ToString(),
                                item.Title,
                                result.Value,
                                result.Count,
                                (total == 0) ? 0 : (result.Count * 100 / total)
                            );
                        }
                    }
#region old
                    //dataTable.Columns.Add("QuestionNumber", typeof(int));
                    //dataTable.Columns.Add("QuestionContent", typeof(string));

                    //dataTable.Columns.Add("AnswerNumber1", typeof(int));
                    //dataTable.Columns.Add("AnswerContent1", typeof(string));
                    //dataTable.Columns.Add("Answer1Count", typeof(int));
                    //dataTable.Columns.Add("Answer1Percentage", typeof(float));

                    //dataTable.Columns.Add("AnswerNumber2", typeof(int));
                    //dataTable.Columns.Add("AnswerContent2", typeof(string));
                    //dataTable.Columns.Add("Answer2Count", typeof(int));
                    //dataTable.Columns.Add("Answer2Percentage", typeof(float));

                    //dataTable.Columns.Add("AnswerNumber3", typeof(int));
                    //dataTable.Columns.Add("AnswerContent3", typeof(string));
                    //dataTable.Columns.Add("Answer3Count", typeof(int));
                    //dataTable.Columns.Add("Answer3Percentage", typeof(float));

                    //dataTable.Columns.Add("AnswerNumber4", typeof(int));
                    //dataTable.Columns.Add("AnswerContent4", typeof(string));
                    //dataTable.Columns.Add("Answer4Count", typeof(int));
                    //dataTable.Columns.Add("Answer4Percentage", typeof(float));

                    //dataTable.Columns.Add("AnswerNumber5", typeof(int));
                    //dataTable.Columns.Add("AnswerContent5", typeof(string));
                    //dataTable.Columns.Add("Answer5Count", typeof(int));
                    //dataTable.Columns.Add("Answer5Percentage", typeof(float));

                    //dataTable.Columns.Add("AnswerNumber6", typeof(int));
                    //dataTable.Columns.Add("AnswerContent6", typeof(string));
                    //dataTable.Columns.Add("Answer6Count", typeof(int));
                    //dataTable.Columns.Add("Answer6Percentage", typeof(float));


                    //for (int i = 0; i < items.Count; i++)
                    //{
                    //    PollItem item = items[i];
                    //    //int totalOptions = item.Option1Count + item.Option2Count + item.Option3Count + item.Option4Count + item.Option5Count + item.Option6Count;
                    //    int optionCount1 = item.EmployeePollItems.Sum(epi => epi.Option1Count);
                    //    int optionCount2 = item.EmployeePollItems.Sum(epi => epi.Option2Count);
                    //    int optionCount3 = item.EmployeePollItems.Sum(epi => epi.Option3Count);
                    //    int optionCount4 = item.EmployeePollItems.Sum(epi => epi.Option4Count);
                    //    int optionCount5 = item.EmployeePollItems.Sum(epi => epi.Option5Count);
                    //    int optionCount6 = item.EmployeePollItems.Sum(epi => epi.Option6Count);

                    //    int totalOptions = optionCount1 + optionCount2 + optionCount3 + optionCount4 + optionCount5 + optionCount6;

                    //    dataTable.Rows.Add(
                    //        (i + 1).ToString(),
                    //        item.Title,
                    //        1,
                    //        item.Option1,
                    //        optionCount1,
                    //        (totalOptions == 0) ? 0 : (optionCount1 * 100 / totalOptions),
                    //        2,
                    //        item.Option2,
                    //        optionCount2,
                    //        (totalOptions == 0) ? 0 : (optionCount2 * 100 / totalOptions),
                    //        3,
                    //        item.Option3,
                    //        optionCount3,
                    //        (totalOptions == 0) ? 0 : (optionCount3 * 100 / totalOptions),
                    //        4,
                    //        item.Option4,
                    //        optionCount4,
                    //        (totalOptions == 0) ? 0 : (optionCount4 * 100 / totalOptions),
                    //        5,
                    //        item.Option5,
                    //        optionCount5,
                    //        (totalOptions == 0) ? 0 : (optionCount5 * 100 / totalOptions),
                    //        6,
                    //        item.Option6,
                    //        optionCount6,
                    //        (totalOptions == 0) ? 0 : (optionCount6 * 100 / totalOptions));
                    //}
#endregion
                    break;
                case 2:
                    //dataTable.Columns.Add("ID", typeof(long));
                    dataTable.Columns.Add("PollTitle", typeof(string));
                    if (!poll.Anonymous)
                    {
                        dataTable.Columns.Add("Username", typeof(string));
                        dataTable.Columns.Add("FullName", typeof(string));
                        dataTable.Columns.Add("DepartmentId", typeof(string));
                        dataTable.Columns.Add("DepartmentName", typeof(string));
                        dataTable.Columns.Add("ManagerId", typeof(string));
                        dataTable.Columns.Add("ManagerName", typeof(string));
                    }
                    else
                    {
                        dataTable.Columns.Add("SessionId", typeof(string));
                    }
                    dataTable.Columns.Add("QuestionNumber", typeof(int));
                    dataTable.Columns.Add("QuestionContent", typeof(string));
                    dataTable.Columns.Add("AnswerNumber", typeof(int));
                    dataTable.Columns.Add("AnswerContent", typeof(string));

                    for (int i = 0; i < poll.PollItems.Count; i++)
                    {
                        PollItem item = poll.PollItems[i];

                        IList<EmployeePollItem> employeeItems = GetSession.QueryOver<EmployeePollItem>()
                            .Where(x => x.PollItem.Id == item.Id).List();

                        foreach (EmployeePollItem employeeItem in employeeItems)
                        {
                            int answerNumber = 0;
                            string answerContent = "";

                            switch (item.Type)
                            {
                                case QuestionType.Open:
                                    answerContent = employeeItem.AnswerText;
                                    break;
                                case QuestionType.Range:
                                    answerNumber = employeeItem.AnswerValue;
                                    break;
                                case QuestionType.Multiple:

                                    if (employeeItem.Option1Count > 0)
                                    {
                                        answerNumber = 1;
                                        answerContent = item.Option1;
                                    }
                                    if (employeeItem.Option2Count > 0)
                                    {
                                        answerNumber = 2;
                                        answerContent = item.Option2;
                                    }
                                    if (employeeItem.Option3Count > 0)
                                    {
                                        answerNumber = 3;
                                        answerContent = item.Option3;
                                    }
                                    if (employeeItem.Option4Count > 0)
                                    {
                                        answerNumber = 4;
                                        answerContent = item.Option4;
                                    }
                                    if (employeeItem.Option5Count > 0)
                                    {
                                        answerNumber = 5;
                                        answerContent = item.Option5;
                                    }
                                    if (employeeItem.Option6Count > 0)
                                    {
                                        answerNumber = 6;
                                        answerContent = item.Option6;
                                    }
                                    break;
                            }

                            if (!poll.Anonymous)
                            {
                                dataTable.Rows.Add(item.Poll.Title,
                                    employeeItem.Employee.Username,
                                    employeeItem.Employee.FullName,
                                    employeeItem.PollTaking != null ? employeeItem.PollTaking.Department_id.ToString() : String.Empty,
                                    employeeItem.Employee.DepartmentName,
                                    employeeItem.PollTaking != null ? (employeeItem.PollTaking.Manager != null ? employeeItem.PollTaking.Manager.Id.ToString() : String.Empty) : String.Empty,
                                    employeeItem.PollTaking != null ? (employeeItem.PollTaking.Manager != null ? employeeItem.PollTaking.Manager.FullName : String.Empty) : String.Empty,
                                    (i + 1).ToString(),
                                    item.Title,
                                    answerNumber,
                                    answerContent);
                            }
                            else
                            {
                                dataTable.Rows.Add(item.Poll.Title,
                                    employeeItem.SessionId.ToString(),
                                    (i + 1).ToString(),
                                    item.Title,
                                    answerNumber,
                                    answerContent);
                            }
                        }
                    }


                    break;
            }

            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            Response.Charset = "UTF-8";

            DataView dataView = new DataView(dataTable);
            switch (type)
            {
                case 1:
                    break;
                case 2:
                    if (!poll.Anonymous)
                    {
                        dataView.Sort = "FullName";
                    }
                    else
                    {
                        dataView.Sort = "SessionId";
                    }
                    break;
            }

            var grid = new GridView()
            {
                DataSource = dataView
            };
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
    }
}
