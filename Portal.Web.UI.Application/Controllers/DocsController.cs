using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.Docs;
using AutoMapper;
using System.IO;
using NHibernate.Transform;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
namespace Portal.Controllers
{
    [Employee]
    public class DocsController : ApplicationController
    {
        [Admin]
        public ActionResult Index()
        {
            var items = GetSession.QueryOver<Doc>()
                .List();

            IEnumerable<Employee> emp = GetSession.QueryOver<Employee>().List();

            ViewBag.emps = emp;

            ViewBag.count = emp.Count();

            return View(items);
        }

        public ActionResult List()
        {
            var items = GetSession.QueryOver<Doc>()
                .List();
            return View(items);
        }



        [HttpPost, Transaction]
        public JavaScriptResult UpdateUsers(DocFormModel model, string[] users, long id)
        {
            string result = "";
            if (users != null)
            {
                if (users.Length > 0)
                {
                    result = string.Join(",", users);
                    //return Redirect("/ForumItems/");
                }
            }



            Doc forumTarget = GetSession.Get<Doc>(id);

            forumTarget.Units = result;

            GetSession.Save(forumTarget);
            string js = @"alert('עודכן בהצלחה')";
            return JavaScript(js);
        }


        public JsonResult getEmps(string[] ids)
        {


            IList<Employee> list = null;


            for (int i = 0; i < ids.Length; i++)
            {
                var emps = GetSession.QueryOver<Employee>()
                .Where(x => x.Department.Id == Convert.ToInt64(ids[0]))
                .List();
                foreach (Employee e in emps)
                {
                    list.Add(e);
                }
                
            }


            //ViewBag.users = new MultiSelectList(list, "Id", "Name");
            //Session["ViewBagusers"] = ViewBag.users;


            var test = list.ToList()
                  .Select(x => new
                  {
                      Name = x.FirstName,
                      Phone = x.Phone,
                      ID = x.Id
                  });


            return Json(test, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public JsonResult GetUsers(string[] ids)
        {

        
            return Json(new
            {
                Users = from u in GetSession.QueryOver<Employee>().List().Where(x => x.Department.Id == Convert.ToUInt32(ids[0]))
                        select new {
                            Name = u.FirstName,
                            ID = u.Id,
                            Lastname = u.LastName 
                        }
            },JsonRequestBehavior.AllowGet);

        }







        [Admin]
        public ActionResult Create()
        {
            //var myCustomerList = GetSession.QueryOver<Employee>().Select(x => x.Department.Id).List<Int64>().Distinct().ToArray();
            ViewBag.deps = new MultiSelectList(GetSession.QueryOver<Department>().List(), "Id", "Name");
            ViewBag.depsIds = GetSession.QueryOver<Department>().Select(x => x.Id);


           // IList<Employee> empsa = (IList<Employee>)Session["ViewBagusers"];

        

            //if (string.IsNullOrEmpty(Session["ids"].ToString()))
            //{

            //    Debug.WriteLine(Session["ids"]);
            //    string[] ids = Session["ids"].ToString().Split(',');
            //    ViewBag.users = new MultiSelectList(GetSession.QueryOver<Employee>().List().
            //         Where(x => x.Department.Id == Convert.ToInt64(ids[0])), "Id", "FirstName");
            //}

            var units = GetSession.QueryOver<Department>()
                      .OrderBy(x => x.Name).Asc
                      .List();
            ViewBag.alex = units.ToList();
            

            var model = new DocFormModel();
            return View(model);
        }

        [Admin, Transaction, HttpPost, ValidateInput(false)]
        public ActionResult Create(DocFormModel model, HttpPostedFileBase upload_file,FormCollection coll)
        {
            string tt = Request.Form["upload_text"];

            if (upload_file != null)
            {
                var uploaded_file = Guid.NewGuid();
                var u_file = upload_file.InputStream;
                int index = upload_file.FileName.IndexOf('.');
                string new_name = uploaded_file + "." + upload_file.FileName.Substring(index + 1);

                var path = Path.Combine(Server.MapPath("~/Public/userfiles/docs"), new_name);
                upload_file.SaveAs(path);
                model.Url = new_name;
            }
            else
            {
                model.Url = Request.Form["upload_text"];
            }


            var item = Mapper.Map<DocFormModel, Doc>(model);





            if (Request.Form["userSS"] != null)
            {
                item.UsersAllowed = Request.Form["userSS"].ToString();
            }


            if (Request.Form["Units[]"] != null)
            {
                item.Units = Request.Form["Units[]"].ToString();
            }
            if (string.IsNullOrEmpty(Request.Form["Units[]"]))
            {
                item.Units = "0";
            }

            if (coll["isAvailable2All"] == "on")
            {
                item.isAvailable2All = true;
            }
            else
            {
                item.isAvailable2All = false;
            }

            item.NotSubmited = GetSession.QueryOver<Employee>().RowCount().ToString();

            item.Submited = "0";
           

          
            GetSession.Save(item);

            return RedirectToAction("Index");
        }

        [Admin]
        public ActionResult Edit(long id)
        {
            ViewBag.allunits = GetSession.QueryOver<Department>().List().Distinct().ToArray();
            var item = GetSession.Get<Doc>(id);
            ViewBag.alex = item.Units;

           // IList<Employee> usersList  = null;
            List<Employee> list = new List<Employee>();

            string[] ids = item.UsersAllowed.Split(',');

            if (item.UsersAllowed != "")
            {
                foreach (var empid in ids)
                    {
                        var em = GetSession.Get<Employee>((long)Convert.ToUInt32(empid));
                        list.Add(em);
                    }
            }



            //var ppl =
            //          GetSession.QueryOver<Employee>()
            //            .Select(x => new
            //            {
            //                EmID = x.Id,
            //                Name = string.Format("{0} {1}", x.FirstName, x.LastName)
            //            }).List();

            //ViewBag.StandID = new SelectList(ppl, "StandID", "Name");

            var stands = GetSession.QueryOver<Employee>().Where(s => s.IsActive == true).List();

            var list2 =
                    from u in list
                    select new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.FirstName + " " + u.LastName
                    };


            ViewBag.users = new MultiSelectList(list2, "Value", "Text");



            ViewBag.deps = new MultiSelectList(GetSession.QueryOver<Department>().List(), "Id", "Name");
          

            ViewBag.docid = id;

            var model = Mapper.Map<Doc, DocFormModel>(item);

            return View(model);
        }

        [Admin, Transaction, HttpPost, ValidateInput(false)]
        public ActionResult Edit(DocFormModel model, HttpPostedFileBase upload_file)
        {
            if (ModelState.IsValid)
            {
                var item = GetSession.Get<Doc>(model.Id);

                if (upload_file != null)
                {
                    var uploaded_file = Guid.NewGuid();
                    var u_file = upload_file.InputStream;
                    int index = upload_file.FileName.IndexOf('.');
                    string new_name = uploaded_file + "." + upload_file.FileName.Substring(index + 1);

                    var path = Path.Combine(Server.MapPath("~/Public/userfiles/docs"), new_name);
                    upload_file.SaveAs(path);
                    model.Url = new_name;
                }


                Mapper.Map<DocFormModel, Doc>(model, item);


                if (Request.Form["UsersListEdit"] != null)
                {
                    item.UsersAllowed = Request.Form["UsersListEdit"].ToString();
                }


                if (Request.Form["Units[]"] != null)
                {
                    item.Units = Request.Form["Units[]"].ToString();
                }
                if (string.IsNullOrEmpty(Request.Form["Units[]"]))
                {
                    item.Units = "0";
                }


                GetSession.Update(item);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Admin, Transaction]
        public ActionResult Destroy(long id)
        {
            var item = GetSession.Get<Doc>(id);
            GetSession.Delete(item);
            return RedirectToAction("Index");
        }

        [Admin, Transaction, HttpPost]
        public ContentResult Active(long id, bool isActive)
        {
            var item = GetSession.Get<Doc>(id);
            item.Active = isActive;
            GetSession.Update(item);
            return Content("ok");
        }


        [Transaction, HttpPost]
        public ContentResult DocAction(string user, long doc)
        {
            var item = GetSession.Get<Doc>(doc);
          
                if (string.IsNullOrEmpty(item.Submited))
                    item.Submited = item.Submited + user;
                else
                    item.Submited = item.Submited + "," + user;

                item.NotSubmited = (GetSession.QueryOver<Employee>().List().Count() - item.Submited.Split(',').Length).ToString();

            GetSession.Update(item);
            return Content("ok");
        }




        [HttpGet]
        public ActionResult ExportToExcelNotSubmited(long id)
        {
            string[] arrayId = new string[] { };
            Doc doc = GetSession.Get<Doc>(id);
            arrayId = doc.Submited.Split(',').ToArray();

         
            IList<Employee> subEmps = new List<Employee>();
            IList<Employee> all = GetSession.QueryOver<Employee>().List();


            IList<Employee> notSubEmps = new List<Employee>();




            foreach (Employee emp in all)
            {
                foreach (var item in arrayId)
                {
                    if (emp.Id == Convert.ToInt64(item))
                    {

                    }
                    else
                    {
                        notSubEmps.Add(emp);
                    }
                }
            }









            var employeesTbl = new System.Data.DataTable("teste");
            employeesTbl.Columns.Add("ID", typeof(long));
            employeesTbl.Columns.Add("Name", typeof(string));
            employeesTbl.Columns.Add("Last name", typeof(string));
            employeesTbl.Columns.Add("Email", typeof(string));


            for (int i = 0; i < notSubEmps.Count; i++)
            {
                employeesTbl.Rows.Add(notSubEmps[i].Id, notSubEmps[i].FirstName, notSubEmps[i].LastName, notSubEmps[i].Email);
            }

            var grid = new GridView();
            grid.DataSource = employeesTbl;
            grid.DataBind();

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=NotSubmited_" + DateTime.Now.ToShortDateString() + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }

        [HttpGet]
        public ActionResult ExportToExcelSubmited(long id)
        {

            string[] arrayId = new string[] { };
            Doc doc = GetSession.Get<Doc>(id);
            arrayId = doc.Submited.Split(',').ToArray();


            if (doc.Submited =="0")
            {
                string js = @"alert('empty list')";
                return Content("<script>alert('hello world')</script>");
                //return new EmptyResult();
            }

            IList<Employee> subEmps = new List<Employee>();

            foreach (var item in arrayId)
            {
                if (item=="0")
                {
                    continue;
                }
                subEmps.Add(GetSession.Get<Employee>(Convert.ToInt64(item)));
            }


            var employeesTbl = new System.Data.DataTable("teste");
            employeesTbl.Columns.Add("ID", typeof(long));
            employeesTbl.Columns.Add("Name", typeof(string));
            employeesTbl.Columns.Add("Last name", typeof(string));
            employeesTbl.Columns.Add("Email", typeof(string));


            for (int i = 0; i < subEmps.Count; i++)
            {
                employeesTbl.Rows.Add(subEmps[i].Id, subEmps[i].FirstName, subEmps[i].LastName, subEmps[i].Email);
            }

            var grid = new GridView();
            grid.DataSource = employeesTbl;
            grid.DataBind();

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=Submited_"+DateTime.Now.ToShortDateString()+".xls");
            Response.ContentType = "application/ms-excel";
            Response.ContentEncoding = System.Text.Encoding.Unicode;
            Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }

        public ActionResult Denied()
        {
            return View();
        }
        
        public ActionResult Show(long? id)
        {
            if (id.HasValue)
            {
                var item = GetSession.QueryOver<Doc>()
                    .Where(x => x.Id == id)
                    .List()
                    .FirstOrDefault();
                if (item == null)
                    return Redirect("404.aspx");

                string currEmpUnit = GetSession.Get<Employee>(GetEmployeeId).Department.Id.ToString();

                string[] arr = item.Units.Split(',').ToArray();

                if (arr.Contains(currEmpUnit)){
                    return View(item);
                
                }
                else
                {
                    return Redirect("666.aspx");
                }
                
                
            }

            return RedirectToAction("Denied", "Docs");
        }










    }
}
