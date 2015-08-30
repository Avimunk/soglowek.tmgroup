using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using Portal.Models.Polls;
using Portal.Entities;
using Portal.Attributes;
using AutoMapper;
using NHibernate.Linq;

namespace Portal.Controllers
{
    [Employee]
    public class PollsController : ApplicationController
    {
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

            //public void InitMembers(ISession session) {

            return View(model);
        }

        [Admin, HttpPost, Transaction, ValidateInput(false)]
        public ActionResult Create(PollFormModel model)
        {

            if (ModelState.IsValid)
            {
                var item = Mapper.Map<PollFormModel, Poll>(model);

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

            bool anonymous = model.Anonymous;
            if (!model.Anonymous && item.Anonymous)
            {
                if (item.PollItems.Any(pi => pi.EmployeePollItems.Count > 0))
                {
                    anonymous = true;
                }
            }

            if (ModelState.IsValid)
            {
                Mapper.Map<PollFormModel, Poll>(model, item);

                model.InitMembers(GetSession);

                item.Anonymous = anonymous;

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

            IList<PollItem> items = GetSession.QueryOver<PollItem>()
                .Where(x => x.Poll.Id == id).List();

            var dataTable = new System.Data.DataTable("Poll");

            switch (type)
            {
                case 1:
                    dataTable.Columns.Add("QuestionNumber", typeof(int));
                    dataTable.Columns.Add("QuestionContent", typeof(string));

                    dataTable.Columns.Add("AnswerNumber1", typeof(int));
                    dataTable.Columns.Add("AnswerContent1", typeof(string));
                    dataTable.Columns.Add("Answer1Count", typeof(int));
                    dataTable.Columns.Add("Answer1Percentage", typeof(float));

                    dataTable.Columns.Add("AnswerNumber2", typeof(int));
                    dataTable.Columns.Add("AnswerContent2", typeof(string));
                    dataTable.Columns.Add("Answer2Count", typeof(int));
                    dataTable.Columns.Add("Answer2Percentage", typeof(float));

                    dataTable.Columns.Add("AnswerNumber3", typeof(int));
                    dataTable.Columns.Add("AnswerContent3", typeof(string));
                    dataTable.Columns.Add("Answer3Count", typeof(int));
                    dataTable.Columns.Add("Answer3Percentage", typeof(float));

                    dataTable.Columns.Add("AnswerNumber4", typeof(int));
                    dataTable.Columns.Add("AnswerContent4", typeof(string));
                    dataTable.Columns.Add("Answer4Count", typeof(int));
                    dataTable.Columns.Add("Answer4Percentage", typeof(float));

                    dataTable.Columns.Add("AnswerNumber5", typeof(int));
                    dataTable.Columns.Add("AnswerContent5", typeof(string));
                    dataTable.Columns.Add("Answer5Count", typeof(int));
                    dataTable.Columns.Add("Answer5Percentage", typeof(float));

                    dataTable.Columns.Add("AnswerNumber6", typeof(int));
                    dataTable.Columns.Add("AnswerContent6", typeof(string));
                    dataTable.Columns.Add("Answer6Count", typeof(int));
                    dataTable.Columns.Add("Answer6Percentage", typeof(float));


                    for (int i = 0; i < items.Count; i++)
                    {
                        PollItem item = items[i];
                        //int totalOptions = item.Option1Count + item.Option2Count + item.Option3Count + item.Option4Count + item.Option5Count + item.Option6Count;
                        int optionCount1 = item.EmployeePollItems.Sum(epi => epi.Option1Count);
                        int optionCount2 = item.EmployeePollItems.Sum(epi => epi.Option2Count);
                        int optionCount3 = item.EmployeePollItems.Sum(epi => epi.Option3Count);
                        int optionCount4 = item.EmployeePollItems.Sum(epi => epi.Option4Count);
                        int optionCount5 = item.EmployeePollItems.Sum(epi => epi.Option5Count);
                        int optionCount6 = item.EmployeePollItems.Sum(epi => epi.Option6Count);

                        int totalOptions = optionCount1 + optionCount2 + optionCount3 + optionCount4 + optionCount5 + optionCount6;

                        dataTable.Rows.Add(
                            (i + 1).ToString(),
                            item.Title,
                            1,
                            item.Option1,
                            optionCount1,
                            (totalOptions == 0) ? 0 : (optionCount1 * 100 / totalOptions),
                            2,
                            item.Option2,
                            optionCount2,
                            (totalOptions == 0) ? 0 : (optionCount2 * 100 / totalOptions),
                            3,
                            item.Option3,
                            optionCount3,
                            (totalOptions == 0) ? 0 : (optionCount3 * 100 / totalOptions),
                            4,
                            item.Option4,
                            optionCount4,
                            (totalOptions == 0) ? 0 : (optionCount4 * 100 / totalOptions),
                            5,
                            item.Option5,
                            optionCount5,
                            (totalOptions == 0) ? 0 : (optionCount5 * 100 / totalOptions),
                            6,
                            item.Option6,
                            optionCount6,
                            (totalOptions == 0) ? 0 : (optionCount6 * 100 / totalOptions));
                    }

                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Response.Charset = "UTF-8";
                    break;
                case 2:
                    if (poll.Anonymous)
                    {
                        return null;
                    }
                    //dataTable.Columns.Add("ID", typeof(long));
                    dataTable.Columns.Add("PollTitle", typeof(string));
                    dataTable.Columns.Add("Username", typeof(string));
                    dataTable.Columns.Add("FullName", typeof(string));
                    dataTable.Columns.Add("DepartmentName", typeof(string));
                    dataTable.Columns.Add("QuestionNumber", typeof(int));
                    dataTable.Columns.Add("QuestionContent", typeof(string));
                    dataTable.Columns.Add("AnswerNumber", typeof(int));
                    dataTable.Columns.Add("AnswerContent", typeof(string));
                    //dataTable.Columns.Add("Option2", typeof(string));
                    //dataTable.Columns.Add("Option3", typeof(string));
                    //dataTable.Columns.Add("Option4", typeof(string));
                    //dataTable.Columns.Add("Option5", typeof(string));
                    //dataTable.Columns.Add("Option6", typeof(string));
                    //dataTable.Columns.Add("Option1Count", typeof(int));
                    //dataTable.Columns.Add("Option2Count", typeof(int));
                    //dataTable.Columns.Add("Option3Count", typeof(int));
                    //dataTable.Columns.Add("Option4Count", typeof(int));
                    //dataTable.Columns.Add("Option5Count", typeof(int));
                    //dataTable.Columns.Add("Option6Count", typeof(int));


                    for (int i = 0; i < items.Count; i++)
                    {
                        PollItem item = items[i];

                        IList<EmployeePollItem> employeeItems = GetSession.QueryOver<EmployeePollItem>()
                            .Where(x => x.PollItem.Id == item.Id).List();

                        foreach (EmployeePollItem employeeItem in employeeItems)
                        {
                            int answerNumber = 0;
                            string answerContent = "";

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

                            dataTable.Rows.Add(item.Poll.Title,
                                employeeItem.Employee.Username,
                                employeeItem.Employee.FullName,
                                employeeItem.Employee.DepartmentName,
                                (i + 1).ToString(),
                                item.Title,
                                answerNumber,
                                answerContent);
                        }
                    }

                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                    Response.Charset = "UTF-8";
                    break;
            }

            var grid = new GridView();
            grid.DataSource = dataTable;
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
