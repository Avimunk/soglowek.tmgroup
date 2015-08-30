using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Models.Misc;
using System.Web.Security;
using QDFeedParser;
using System.Net;
using Portal.Attributes;
using System.Xml;
using Portal.Entities;
using LinqToExcel;
using Portal.Models.Employees;
using AutoMapper;
using System.Security.Principal;
using Portal.Models.JobTitles;
using Portal.Models.Departments;
using System.Net.Mail;
using System.Text;


using Portal.Clients;
using System.IO;
using System.Data;
using System.Web.Hosting;
using System.Web.UI.WebControls;
using System.Web.UI;


namespace Portal.Controllers
{
    
    public class MiscController : ApplicationController
    {
        public ActionResult Login()
        {
            string[] allowed = MvcApplication.Config("allowed").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //if (!allowed.Contains(HttpContext.Request.UserHostAddress.ToString()))
            //{
            //    Response.Redirect("http://www.shekelgroup.co.il/");
            //}

            //string[] temp = Convert.ToString(WindowsIdentity.GetCurrent().Name).Split('\\');
            //ViewBag.login_name = temp[1];

            var model = new LoginFormModel();

            //SSO 
            string ssoConfig = MvcApplication.Config("sso.enabled");

            if (!String.IsNullOrEmpty(ssoConfig))
            {
                bool ssoEnabled = bool.Parse(ssoConfig);
                if (ssoEnabled)
                {
                    using (HostingEnvironment.Impersonate())
                    {
                        SSOClient ssoClient = new SSOClient();

                        //string ssoLoginName = ssoClient.GetCurrentLoginName();
                        string ssoLoginName = User.Identity.Name;

                        if (ssoLoginName.Contains("\\"))
                        {
                            ssoLoginName = ssoLoginName.Split('\\')[1];
                        }

                        if (!String.IsNullOrEmpty(ssoLoginName))
                        {
                            string ssoPropertyName = MvcApplication.Config("sso.id_property");

                            if (!String.IsNullOrEmpty(ssoPropertyName))
                            {
                                string ssoPropertyValue = ssoClient.GetProperty(ssoLoginName, ssoPropertyName);

                                if (!String.IsNullOrEmpty(ssoPropertyValue))
                                {
                                    //Int64 localId = 2065;
                                    Int64 localId = Int64.Parse(ssoPropertyValue);

                                    if (localId > 0)
                                    {
                                        var emp = GetSession.QueryOver<Employee>().Where(x => x.Id == localId).SingleOrDefault();
                                        if (emp != null)
                                        {
                                            if (String.IsNullOrWhiteSpace(emp.Email) || 
                                                emp.Email.Equals("nomail@mail.com", StringComparison.InvariantCultureIgnoreCase) || 
                                                String.IsNullOrWhiteSpace(emp.Username))
                                            {
                                                return View(new LoginFormModel());
                                            }
                                            else
                                            {
                                                return Login(new LoginFormModel()
                                                {
                                                    Username = emp.Email,
                                                    Password = emp.Password
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return View(new LoginFormModel());

        }

        [HttpPost]
        public ActionResult Login(LoginFormModel model)
        {




            if (ModelState.IsValid)
            {
                var emp = GetSession.QueryOver<Employee>().Where(x => x.Email == model.Username && x.Password == model.Password && x.IsActive == true).SingleOrDefault();
                /*
                if (emp != null)
                {
                    if (emp.IsAdmin)
                    {

                    }
                    else
                    {


                    }
                }
                */

                //return Content("Password:" + model.Password + "Username: " + model.Username);

                var adm = GetSession.QueryOver<Employee>().Where(x => x.Username == model.Username).SingleOrDefault();
                if (emp != null)
                {
                    //if (HttpContext.Request.Path == "/login" && emp.IsAdmin == false)
                    //{
                    //    return Redirect("/");
                    //}
                    FormsAuthentication.SetAuthCookie(emp.Username, false);
                    //add cookie for auto login next time
                    Response.Cookies["user"].Value = model.Username;
                    Response.Cookies["user"].Expires = DateTime.MaxValue;
                    Response.Cookies["password"].Value = model.Password;
                    Response.Cookies["password"].Expires = DateTime.MaxValue;
                    Response.Cookies["user_name"].Value = emp.Username;
                    Response.Cookies["user_name"].Expires = DateTime.MaxValue;
                    return Redirect(FormsAuthentication.GetRedirectUrl(emp.Username, false));

                }
                else if (adm != null)
                {
                    if (model.Username == MvcApplication.Config("admin.Username") && model.Password == MvcApplication.Config("admin.Password"))
                    {
                        Response.Cookies["user"].Value = model.Username;
                        Response.Cookies["user"].Expires = DateTime.MaxValue;
                        Response.Cookies["password"].Value = model.Password;
                        Response.Cookies["password"].Expires = DateTime.MaxValue;
                        Response.Cookies["user_name"].Value = model.Username;
                        Response.Cookies["user_name"].Expires = DateTime.MaxValue;
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, false));
                    }
                }
            }


            HttpCookie currentUserCookie = Request.Cookies["user"];
            if (currentUserCookie != null)
            {
                Response.Cookies.Remove("user");
                Response.Cookies.Remove("password");
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                Response.SetCookie(currentUserCookie);
            }

            return View(model);
        }

        public ActionResult Logout()
        {

            HttpCookie currentUserCookie = Request.Cookies["user"];
            Response.Cookies.Remove("user");
            Response.Cookies.Remove("password");
            currentUserCookie.Expires = DateTime.Now.AddDays(-10);
            currentUserCookie.Value = null;
            Response.SetCookie(currentUserCookie);

            Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

        [OutputCache(Duration = 900)]
        public ActionResult GetRss()
        {

            try
            {
                var factory = new HttpFeedFactory();

                var url = new Uri(MvcApplication.Config("home.Rss"));
                var item = factory.CreateFeed(url);

                return View(item);
            }
            catch
            {
                return Content("");
            }
        }


      
        public ActionResult Error500()
        {
            return View();
     
        }









        [Transaction, HttpGet]
        public ActionResult UpdateEmployeesPhotos()
        {




            var items = GetSession.QueryOver<Employee>().List();

            int sum = 0;
            foreach (Employee item in items)
            {
                var emp = GetSession.Get<Employee>(item.Id);
                emp.Picture = emp.Id.ToString() + ".jpg";
                if (emp.BDay == null)
                {
                    emp.BDay = DateTime.Now;
                }
                GetSession.Update(item);
                sum += 1;
            }
            return Content("done : updated -> " + sum);

        }


        public ContentResult test()
        {








            return Content("dddd");
        }


        [Transaction]
        public ActionResult UpdateEmployees()
        {



            ////-----------------
            //string filePaths = @"E:\Users\max\Desktop\Development\ON AIR SITES\New Sites Online_Alex\Zoglobek PORTAL\Source\Portal.Web.UI.Application\Public\userfiles\SOGPORTAL.xlsx";


            //string strFile = @"E:\Users\max\Desktop\Development\ON AIR SITES\New Sites Online_Alex\Zoglobek PORTAL\Source\Portal.Web.UI.Application\Public\userfiles\SOGPORTAL.xlsx";

            //string strTemp = Path.GetExtension(strFile).ToLower();

            //if (strTemp == ".xls")
            //{
            //    strFile = Path.ChangeExtension(strFile, "xlsx");
            //}





            //foreach (string myfile in filePaths)
            //{
            //    filename = Path.ChangeExtension(myfile, ".txt");
            //    System.IO.File.Move(myfile, filename);
            //}


            ////------------------



            Log newLog = new Log();
            newLog.id = Guid.NewGuid();


            int added = 0;
            int deleted = 0;
            int updated = 0;
            int errors = 0;
            int warnings = 0;
            string result = "";


            int logg = 0;
            string name="";
            try
            {
                var excel = new ExcelQueryFactory();

                string[] ex_names = MvcApplication.Config("excel_name").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


               

                //insert all employees from excel to temp list<>
                List<string> excel_emp_id = new List<string>();



                for (int ex_name = 0; ex_name < ex_names.Length; ex_name++)
                {
                    excel.FileName = ex_names[ex_name];

                    //employees from excel
                    var result_employees = from c
                                    in excel.WorksheetNoHeader(0)
                                           select c;



                    List<Employee> employees = new List<Employee>();




                    UpdateJobs();
                    UpdateDepartments();

                    //Dictionary
                    Dictionary<string, long> dep_dict = new Dictionary<string, long>();


                    //existing Department from db
                    List<Department> existing_deps = (List<Department>)GetSession.QueryOver<Department>().List();


                    foreach (Department dep in existing_deps)
                    {
                        dep_dict.Add(dep.Name, dep.Id);
                    }


                    //get all employees to dictinary
                    Dictionary<string, long> emps_dict = new Dictionary<string, long>();



                    Dictionary<string, long> job_dict = new Dictionary<string, long>();

                    //existing Jobs from db
                    List<JobTitle> existing_jobs = (List<JobTitle>)GetSession.QueryOver<JobTitle>().List();

                    foreach (JobTitle job in existing_jobs)
                    {
                        job_dict.Add(job.Name, job.Id);
                    }


                    Dictionary<string, string> users_managers = new Dictionary<string, string>();

                    //result_employees -> employees from excel
                    int i = 0;
                    foreach (var employee in result_employees)
                    {
                        //check for must feilds
                        if (!string.IsNullOrEmpty(employee[1].ToString().Trim())
                            && !string.IsNullOrEmpty(employee[2].ToString().Trim())
                            && !string.IsNullOrEmpty(employee[13].ToString().Trim())
                            && !string.IsNullOrEmpty(employee[0].ToString().Trim()))
                        {


                            logg++;
                            name = employee[0];


                            if (i > 0 && !string.IsNullOrEmpty(employee[0].ToString().Trim()))
                            {

                                //check for wanted feilds
                                if (string.IsNullOrEmpty(employee[5].ToString().Trim()) || string.IsNullOrEmpty(employee[7].ToString().Trim()))
                                {
                                    newLog.WarningLines = newLog.WarningLines + (i + 1) + ",";
                                    warnings++;
                                }

                                excel_emp_id.Add(employee[0].ToString().Trim());

                                if (employee[5] != "")
                                {
                                    users_managers.Add(employee[0], employee[5]);
                                }

                               


                                //if employee exists in DB
                                if (GetSession.Get<Employee>(Convert.ToInt64(employee[0].ToString().Trim())) == null)
                                {
                                    //add new employee
                                    string sBirthDate = employee[7].ToString().Trim() ?? "";
                                    //string username = employee[9].ToString().Trim() == "" ?
                                      //  "New User" : employee[9].ToString().Trim().Substring(0, employee[9].ToString().Trim().ToString().IndexOf('@'));
                                    string userName = employee[0].ToString().Trim();


                                    EmployeeFormModel model = new EmployeeFormModel()
                                    {
                                        Id = Convert.ToInt64(employee[0].ToString().Trim()),
                                        Password = employee[0].ToString().Trim(),
                                        LastName = employee[2].ToString().Trim(),
                                        FirstName = employee[1].ToString().Trim(),
                                        Email = string.IsNullOrEmpty(employee[9].ToString().Trim()) ? "nomail@mail.com" : employee[9].ToString().Trim(),
                                        Phone = employee[10].ToString().Trim(),
                                        Picture = employee[0].ToString().Trim() + ".jpg",
                                        Mobile = employee[11].ToString().Trim(),
                                        //Manager = GetSession.Get<Employee>(Convert.ToInt64(employee[5].ToString().Trim())),
                                        Range = employee[12].ToString().Trim(),
                                        BDay = (employee[7].ToString().Trim() != "") ? Convert.ToDateTime(sBirthDate) : DateTime.Now,
                                        IsActive = Convert.ToBoolean(Convert.ToInt32(employee[13].ToString().Trim())),
                                        JbTitle = GetSession.Get<JobTitle>(job_dict.FirstOrDefault(x => x.Key == employee[8].ToString().Trim()).Value),
                                        Department = GetSession.Get<Department>(dep_dict.FirstOrDefault(x => x.Key == employee[4].ToString().Trim()).Value),
                                        Username = userName
                                    };

                                    var item = Mapper.Map<EmployeeFormModel, Employee>(model);

                                    GetSession.Save(item);
                                    added++;

                                    // string s = job_dict.FirstOrDefault(employee[3].ToString().Trim()).Key;
                                }
                                else
                                {
                                    //update employee

                                    var item = GetSession.Get<Employee>(Convert.ToInt64(employee[0].ToString().Trim()));
                                    var model = Mapper.Map<Employee, EmployeeFormModel>(item);
                                    string sBirthDate = employee[7].ToString().Trim().ToString();

                                    string userName = employee[0].ToString().Trim();


                                    model.Password = employee[0].ToString().Trim();
                                    model.LastName = employee[2].ToString().Trim();
                                    model.FirstName = employee[1].ToString().Trim();
                                    model.Email = string.IsNullOrEmpty(employee[9].ToString().Trim()) ? "nomail@mail.com" : employee[9].ToString().Trim();
                                    model.Phone = employee[10].ToString().Trim();
                                    model.Mobile = employee[11].ToString().Trim();
                                    model.Picture = employee[0].ToString().Trim() + ".jpg";
                                    model.Range = employee[12].ToString().Trim();
                                    //model.Manager = GetSession.Get<Employee>(Convert.ToInt64(employee[5].ToString().Trim()));
                                    model.IsActive = Convert.ToBoolean(Convert.ToInt32(employee[13].ToString().Trim()));
                                    model.BDay = (employee[7].ToString().Trim() != "") ? Convert.ToDateTime(sBirthDate) : DateTime.Now;
                                    model.JbTitle = GetSession.Get<JobTitle>(job_dict.FirstOrDefault(x => x.Key == employee[8].ToString().Trim()).Value);
                                    model.Department = GetSession.Get<Department>(dep_dict.FirstOrDefault(x => x.Key == employee[4].ToString().Trim()).Value);
                                    //model.Username = employee[9].ToString().Trim() == "" ? "" : employee[9].ToString().Trim().ToString().Substring(0, employee[9].ToString().Trim().ToString().IndexOf('@'));
                                    model.Username = userName;
                                    Mapper.Map<EmployeeFormModel, Employee>(model, item);
                                    GetSession.Update(item);
                                    updated++;
                                }
                            }
                            i++;

                        }
                        else
                        {
                            newLog.ErrorLines = newLog.ErrorLines + (i + 1) + ",";
                            errors++;
                            i++;
                        }

                    }



                 

                  
                     //TODO
                        //update managers
                         //UpdateManagers(users_managers);
                        
                   



                }




                // bring all existing employees
                List<Employee> current_employees = (List<Employee>)GetSession.QueryOver<Employee>().List();
                foreach (Employee curr_emp in current_employees)
                {
                    // if employee nof found in excel > disactivate this employee
                    if (!excel_emp_id.Contains(curr_emp.Id.ToString()))
                    {

                        if (curr_emp.Username != MvcApplication.Config("admin.Username"))
                        {
                            var item = GetSession.Get<Employee>(curr_emp.Id);
                            var model = Mapper.Map<Employee, EmployeeFormModel>(item);
                            model.IsActive = false;
                            Mapper.Map<EmployeeFormModel, Employee>(model, item);
                            GetSession.Update(item);

                            deleted++;
                        }
                    }

                    #region 1

                    //if (curr_emp.Username == MvcApplication.Config("admin.Username"))
                    //{
                    //    var item = GetSession.Get<Employee>(curr_emp.Id);
                    //    var model = Mapper.Map<Employee, EmployeeFormModel>(item);


                    //    model.Username = "User";
                    //    model.IsAdmin = true;
                    //    model.IsActive = true;

                    //    Mapper.Map<EmployeeFormModel, Employee>(model, item);
                    //    GetSession.Update(item);
                    //} 
                    #endregion
                }

            




            }
            catch (Exception ex)
            {
                return Content(ex.Data + ">>>>>" + ex.InnerException + ">>>>>" + ex.Message + " >>>>>>>> line" + logg + "name =" + name);
            }
            result = string.Format("<h3>Update Results</h3><p>New : {0} </p><p>Processed : {1} </p><p>Deleted {2}</p><p>Errors {3}</p><p>Warnings {4}</p>", added, updated, deleted, errors, warnings);


            newLog.Logdate = DateTime.Now;
            newLog.UpdatedCount = updated;
            newLog.NewEmps = added.ToString();
            GetSession.Save(newLog);

            TempData["warnings"] = newLog.WarningLines;


            return RedirectToAction("ProssesResult", new { added = added, updated = updated, deleted = deleted, errors = errors, warningsL = newLog.WarningLines, warnings = warnings });
            //return Content(result, "text/html");
        }

        [Transaction]
        public string UpdateManagers(Dictionary<string,string> user_manager)
        {

            Employee temp = new Employee();

            try
            {
                foreach (var listitem in user_manager)
                {
                    if (!string.IsNullOrEmpty(listitem.Key) && !string.IsNullOrEmpty(listitem.Value.Trim()))
                    {
                        
                            var item = GetSession.Get<Employee>(Convert.ToInt64(listitem.Key));
                            var item2 = GetSession.Get<Employee>(Convert.ToInt64(listitem.Value));

                            if (item !=null)
                            {
                                item.Manager = item2;
                                GetSession.Update(item);
                            }

                            
                    }


                   
                    
                }
                return "yes";
            }
            catch (Exception)
            {

                return "no";
            }

         
           
        }


        public ActionResult ProssesResult(string added, int updated, string deleted, string errors, string warningsL, string warnings)
        {
            Log log = new Log()
            {

                WarningLines = warningsL,
                ErrorLines = errors,
                Added = added,
                Deleted = deleted,
                UpdatedCount = updated

            };
            ViewBag.warningCount = warnings;

            try
            {
                SendMailUpdateAdmin(warningsL);
                return View(log);
            }
            catch (Exception ex)
            {

                return Content("Cannot send mail to admin due to lack of internet connection, mail uses gmail account, error = ", ex.Message);
            }


        }



        public ActionResult SendMailUpdateAdmin(string warnings)
        {

            var m = new MailMessage();

            m.IsBodyHtml = false;
            m.BodyEncoding = Encoding.GetEncoding(1255);
            m.SubjectEncoding = Encoding.GetEncoding(1255);
            m.To.Add(MvcApplication.Config("lead.AdminUpdate"));

            m.Subject = Request.ServerVariables["HTTP_HOST"];
            m.Priority = MailPriority.High;
            m.Body = "אין להשיב למייל זה" + "\n" +
                "נושא: תוצעות ריצה\n " +
                 "ישנן שורות בקובץ  - שדות -> ת.לידה,מנהל ישיר חסרים " +
                "שורות: " + warnings + "\n" +

                "תאריך: " + DateTime.Now;

            new SmtpClient().Send(m);
            return Content("Sent");
        }





        public ActionResult SendBdayMessage()
        {
            var items = GetBdayEmployees();

            foreach (var item in items)
            {
                var sentCount = 0;
                var manager = GetSession.Get<Employee>(Convert.ToInt64(item.Manager));
                while (manager != null && sentCount < 2)
                {
                    if (manager.Email.Contains("@"))
                    {
                        new NotificationsController().Bday(manager, item).Deliver();
                        sentCount++;
                        Response.Write(manager.Email + ";");
                    }
                    else
                    {
                        Response.Write(manager.Id + ";");
                    }
                    manager = GetSession.Get<Employee>(manager.Manager);
                }
                Response.Write("\n");
            }

            return Content("sent");
        }

        public void UpdateJobs()
        {

            var excel = new ExcelQueryFactory();

            string[] ex_names = MvcApplication.Config("excel_name").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int ex_name = 0; ex_name < ex_names.Length; ex_name++)
            {
                excel.FileName = ex_names[ex_name];



                var result_employees = from c
                                in excel.WorksheetNoHeader(0)
                                       select c;


                List<JobTitle> existing_jobs = (List<JobTitle>)GetSession.QueryOver<JobTitle>().List();
                List<string> existing_jobs_list = new List<string>();

                foreach (var job in existing_jobs)
                {
                    if (!existing_jobs_list.Contains(job.Name))
                    {
                        existing_jobs_list.Add(job.Name);

                    }
                }


                foreach (var new_job in result_employees)
                {
                    if (!string.IsNullOrEmpty(new_job[8].ToString().Trim()) && !existing_jobs_list.Contains(new_job[8].ToString().Trim()))
                    {
                        JobTitleFormModel model = new JobTitleFormModel()
                        {
                            Name = new_job[8].ToString().Trim()
                        };

                        var item = Mapper.Map<JobTitleFormModel, JobTitle>(model);
                        GetSession.Save(item);
                        existing_jobs_list.Add(new_job[8].ToString().Trim());
                    }
                }


            }
        }

        public void UpdateDepartments()
        {

            var excel = new ExcelQueryFactory();
            string[] ex_names = MvcApplication.Config("excel_name").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int ex_name = 0; ex_name < ex_names.Length; ex_name++)
            {
                excel.FileName = ex_names[ex_name];

                var result_employees = from c
                                in excel.WorksheetNoHeader(0)
                                       select c;


                List<Department> existing_deps = (List<Department>)GetSession.QueryOver<Department>().List();
                List<string> existing_dep_list = new List<string>();

                foreach (var dep in existing_deps)
                {
                    if (!existing_dep_list.Contains(dep.Name))
                    {
                        existing_dep_list.Add(dep.Name);

                    }
                }


                foreach (var new_dep in result_employees)
                {
                    if (!string.IsNullOrEmpty(new_dep[4].ToString().Trim()) && !existing_dep_list.Contains(new_dep[4].ToString().Trim()))
                    {
                        DepartmentFormModel model = new DepartmentFormModel()
                        {
                            Name = new_dep[4].ToString().Trim()
                        };

                        var item = Mapper.Map<DepartmentFormModel, Department>(model);
                        GetSession.Save(item);
                        existing_dep_list.Add(new_dep[4].ToString().Trim());
                    }
                }

            }
        }

        public ActionResult GetLead(string about, string msg)
        {

            var m = new MailMessage();

            m.IsBodyHtml = false;
            m.BodyEncoding = Encoding.GetEncoding(1255);
            m.SubjectEncoding = Encoding.GetEncoding(1255);
            m.To.Add(MvcApplication.Config("lead.Email"));

            m.Subject = Request.ServerVariables["HTTP_HOST"];
            m.Priority = MailPriority.High;
            m.Body = "אין להשיב למייל זה" + "\n" +
                "נושא: " + about + "\n" +
                "הודעה: " + msg + "\n" +
                "תאריך: " + DateTime.Now;

            new SmtpClient().Send(m);
            return Content("Sent");
        }

        [Employee]
        public ActionResult FeedbackToManager(bool isSent = false)
        {
            ViewBag.IsSent = isSent;
            return View();
        }

        [HttpPost]
        public ActionResult FeedbackToManager(string feedback)
        {

            SendEmail(MvcApplication.Config("email.FeedbackToManager", "maxokunev@gmail.com"), "כתוב למנכ\"ל", feedback);

            return RedirectToAction("FeedbackToManager", new { isSent = true });
        }






        public ActionResult ActiveForderUpload()
        {
            var item = GetSession.Get<Setting>((long)1);
            ViewBag.ActiveForderUpload = item.ActiveForderUpload;
            return View();
        }

        [Admin, Transaction, HttpPost]
        public ActionResult ActiveForderUploadActive(bool isActive)
        {
            var item = GetSession.Get<Setting>((long)1);
            item.ActiveForderUpload = isActive;
            GetSession.Update(item);
            return Content("ok");



        }



        public ActionResult TestSSO()
        {

            var item = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //var item = User.Identity.Name;
            //var item = System.Web.HttpContext.Current.User.Identity.Name;

            //             if (item.Contains("\\"))
            //             {
            //                 item = item.Split('\\')[1];
            //             }


            return Content(item);
        }


    }
}
