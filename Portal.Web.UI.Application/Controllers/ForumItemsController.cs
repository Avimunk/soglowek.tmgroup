using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Models.Forums;
using AutoMapper;
using MvcContrib.Pagination;
using NHibernate.Criterion;
using System.Configuration;



namespace Portal.Controllers
{
    [Employee]
    public class ForumItemsController : ApplicationController
    {
        string cs = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;



        public ActionResult ForumUsers(long forumId)
        {


            var item = GetSession.Get<ForumItem>(forumId);

            //#region ado.net
            //string theTitle = Request.Form["Title"];
            //long theAdmin = Convert.ToInt32(Request.Form["Admin"]);
            //bool theActive = (Request.Form["Active"] == "on") ? true : false;
            //SqlConnection sqlConnection1 = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandText = "select Admin_Id from portal_ForumItem where id =" + forumId;
            //cmd.CommandType = CommandType.Text;
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Open();
            //string forumAdminId = "";
            //SqlDataReader dr = cmd.ExecuteReader();
            //if (dr.Read())
            //{
            //    forumAdminId = dr[0].ToString();
            //}
            //sqlConnection1.Close();
            //#endregion


            var users = GetSession.QueryOver<Employee>()
             .OrderBy(x => x.FirstName).Desc
             .List();



            var items = GetSession.Get<ForumItem>(forumId).ForumUsers;
            ViewBag.ForumUsers = items;

            ViewBag.currForum = GetSession.Get<ForumItem>(forumId);

            return View(users);
        }



        public ActionResult Index()
        {
            var items = GetSession.QueryOver<ForumItem>().List();
            //var users = GetSession.QueryOver<Employee>().List();





            var users = GetSession.QueryOver<Employee>()
               .OrderBy(x => x.FirstName).Desc
               .List();

            ViewBag.allusers = users.ToList();
            return View(items);
        }
        [HttpGet, Transaction]
        public ActionResult Delete(long id)
        {
            var item = GetSession.Get<ForumItem>(id);
            GetSession.Delete(item);


            var forumPosts = GetSession.QueryOver<Forum>().Where(x => x.CategoryId == id).List();
            foreach (var post in forumPosts)
            {
                GetSession.Delete(post);
            }


            return RedirectToActionPermanent("Index");
        }

        public ActionResult Create()
        {
            var users = GetSession.QueryOver<Employee>().List();
            ViewBag.users = users;
            var model = new ForumItemModel();
            return View(model);

            ViewBag.list = GetSession.QueryOver<ForumItem>().List();
        }

        [HttpPost, Transaction]
        public ActionResult Create(ForumItemModel model)
        {
            
            var results = Request.Form["ForumUsers[]"];
            long adminId;

            if (!string.IsNullOrEmpty(Request.Form["Admin"]))
            {
                adminId = Convert.ToInt64(Request.Form["Admin"]);
            }
            else
            {
                adminId = GetEmployeeId;
            }

            ForumItem newForum = new ForumItem()
            {

                Title = !string.IsNullOrEmpty(Request.Form["Title"]) ? Request.Form["Title"] : " פורום חדש",
                Active = (Request.Form["Active"] == "on") ? true : false,
                ForumUsers = Request.Form["ForumUsers[]"],



                Admin = GetSession.Get<Employee>(adminId)



            };

            GetSession.Save(newForum);
            #region ado.net



            //string theTitle = Request.Form["Title"];
            //long theAdmin = Convert.ToInt32(Request.Form["Admin"]);
            //bool theActive = (Request.Form["Active"] == "on") ? true : false;
            //SqlConnection sqlConnection1 = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand();
            //Int32 rowsAffected;
            //cmd.CommandText = "INSERT into portal_ForumItem(Title,Active,Admin_id,ForumUsers) values ('" + theTitle + "', '" + theActive + "', " + theAdmin + ", '" + results + "')";
            //cmd.CommandType = CommandType.Text;
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Open();
            //rowsAffected = cmd.ExecuteNonQuery();
            //sqlConnection1.Close(); 
            #endregion
            return Redirect("/ForumItems/");
        }

        [HttpPost, Transaction]
        public ActionResult Update(ForumItemModel model)
        {




            ForumItem forumTarget = GetSession.Get<ForumItem>(Convert.ToInt64(Request.Form["Id"]));

            forumTarget.Admin = GetSession.Get<Employee>(Convert.ToInt64(Request.Form["Admin"]));
            forumTarget.Title = Request.Form["Title"];
            forumTarget.Active = (Request.Form["Active"] == "on") ? true : false;

            GetSession.Update(forumTarget);


            #region ADO.NET
            //string theTitle = Request.Form["Title"];
            //long theAdmin = Convert.ToInt32(Request.Form["Admin"]);
            //long theId = Convert.ToInt32(Request.Form["Id"]);
            //bool theActive = (Request.Form["Active"] == "on") ? true : false;
            //SqlConnection sqlConnection1 = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand();
            //Int32 rowsAffected;
            //cmd.CommandText = "UPDATE portal_ForumItem SET Title = '" + theTitle + "', Admin_id = " + theAdmin + ", Active = '" + theActive + "' WHERE Id = " + theId + "";
            ////cmd.CommandText = "UPDATE portal_ForumItem SET Title = '" + theTitle + "', Admin_id = " + theAdmin + ", Active = '" + theActive + "' WHERE Id = "+theId+"";
            //cmd.CommandType = CommandType.Text;
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Open();
            //rowsAffected = cmd.ExecuteNonQuery();
            //sqlConnection1.Close();

            #endregion

            return Redirect("/ForumItems/");
        }

        [HttpPost, Transaction]
        public JavaScriptResult UpdateUsers(ForumItemModel model, string[] users, long id)
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



            ForumItem forumTarget = GetSession.Get<ForumItem>(id);

            forumTarget.ForumUsers = result;

            GetSession.Save(forumTarget);

            #region ado.net
            //string theTitle = Request.Form["Title"];
            //long theAdmin = Convert.ToInt32(Request.Form["Admin"]);
            //long theId = Convert.ToInt32(Request.Form["Id"]);
            //bool theActive = (Request.Form["Active"] == "on") ? true : false;
            //SqlConnection sqlConnection1 = new SqlConnection(cs);
            //SqlCommand cmd = new SqlCommand();
            //Int32 rowsAffected;
            //cmd.CommandText = "UPDATE portal_ForumItem SET Title = '" + theTitle + "', Admin_id = " + theAdmin + ", Active = '" + theActive + "', ForumUsers = '" + result + "' WHERE Id = " + theId + "";
            ////cmd.CommandText = "UPDATE portal_ForumItem SET Title = '" + theTitle + "', Admin_id = " + theAdmin + ", Active = '" + theActive + "' WHERE Id = "+theId+"";
            //cmd.CommandType = CommandType.Text;
            //cmd.Connection = sqlConnection1;
            //sqlConnection1.Open();
            //rowsAffected = cmd.ExecuteNonQuery();
            //sqlConnection1.Close(); 
            #endregion
            //return RedirectToAction("index");
            string js = @"alert('עודכן בהצלחה')";
            return JavaScript(js);
        }

         [HttpPost, Transaction]
        public JavaScriptResult UpdateAdmin(long forumId,long adminId)
        {           
            ForumItem forumTarget = GetSession.Get<ForumItem>(forumId);

            Employee temp = GetSession.Get<Employee>(adminId);
            forumTarget.Admin = temp;

            GetSession.Save(forumTarget);
            string js = @"alert('מנהל פורום עודכן בהצלחה!')";
            return JavaScript(js);
        }


         [HttpPost, Transaction]
         public ActionResult UpdateTitle(long id, string newTitle)
         {
             ForumItem forumTarget = GetSession.Get<ForumItem>(id);
             forumTarget.Title = newTitle;

             GetSession.Save(forumTarget);
             return Redirect("/ForumItems/");
         }


         [HttpPost, Transaction]
         public ActionResult ForumActiveUpdate(long id, bool isActive)
         {
             ForumItem forumTarget = GetSession.Get<ForumItem>(id);
             forumTarget.Active = isActive;

             GetSession.Save(forumTarget);
             return Redirect("/ForumItems/");
         }



         [HttpPost, Transaction]
         public ActionResult ForumAllowAllUpdate(long id, bool isAvailableTooAll)
         {
             ForumItem forumTarget = GetSession.Get<ForumItem>(id);
             forumTarget.isAvailableTooAll = isAvailableTooAll;

             GetSession.Save(forumTarget);
             return Redirect("/ForumItems/");
         }



      
         public ActionResult FList()
         {
             var items = GetSession.QueryOver<ForumItem>().List();

             return View(items);
         }




    }

}
