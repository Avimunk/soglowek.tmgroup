using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Portal.Attributes;
using Portal.Entities;
using Portal.Helpers;
using Portal.Models.Pages;
using AutoMapper;
using NHibernate.Linq;
using NHibernate.Criterion;

namespace Portal.Controllers
{
    [Employee]
    public class PagesController : ApplicationController
    {
        [Admin]
        public ActionResult Index(PageIndexModel model)
        {



            var items = GetSession.QueryOver<Page>()
                .Where(x => x.SectionId == model.SectionId);

            if (model.ParentId.HasValue)
            {
                var item = GetSession.Get<Page>(model.ParentId);
                model.Title = item.Name;

                if (item.Parent != null)
                {
                    model.PrevParentId = item.Parent.Id;
                }

                items = items.Where(x => x.Parent == item);
            }
            else
            {
                items = items.Where(x => x.Parent == null);
            }

            model.Pages = items.OrderBy(x => x.PageOrder).Asc.List();

            return View(model);
        }

        [Admin]
        public ActionResult Create(long? parentId, int sectionId = 1)
        {
            var model = new PageFormModel()
            {
                ParentId = parentId,
                SectionId = sectionId,
                TypeId = 2
            };
            return View(model);
        }

        [Admin, Transaction, HttpPost, ValidateInput(false)]
        public ActionResult Create(PageFormModel model,FormCollection coll)
        {




            var item = Mapper.Map<PageFormModel, Page>(model);
            Page parent = null;

            if (model.ParentId.HasValue)
            {
                parent = GetSession.Get<Page>(model.ParentId);
                item.Parent = parent;
            }

            int maxOrder = GetSession.Query<Page>()
                .Where(x => x.Parent == parent
                    && x.SectionId == model.SectionId
                ).Max(x => (int?)x.PageOrder) ?? 0;

            if (item.TypeId == 999)
            {
                item.PageTemplate = "tv";
                item.Iframelink1DelayTime = coll["Iframelink1"].ToString().Split(',')[1];
                item.Iframelink2DelayTime = coll["Iframelink2"].ToString().Split(',')[1];
                item.Iframelink3DelayTime = coll["Iframelink3"].ToString().Split(',')[1];
                item.Iframelink4DelayTime = coll["Iframelink4"].ToString().Split(',')[1];
                item.Iframelink5DelayTime = coll["Iframelink5"].ToString().Split(',')[1];           
            }
            if (item.TypeId == 001)
            {
        
            }


            item.PageOrder = maxOrder + 1;
            item.IsInternal = true;
            item.UpdatedDate = DateTime.Now;

            GetSession.Save(item);

            return RedirectToAction("Index", new { model.ParentId, model.SectionId });
        }

        [Admin]
        public ActionResult Edit(long id)
        {
            var item = GetSession.Get<Page>(id);
            ViewBag.settings = GetSession.Get<Setting>((long)1);
            var model = Mapper.Map<Page, PageFormModel>(item);

            return View(model);
        }

        [Admin, Transaction, HttpPost, ValidateInput(false)]
        public ActionResult Edit(PageFormModel model)
        {
            if (ModelState.IsValid)
            {
                

                var item = GetSession.Get<Page>(model.Id);

                Mapper.Map<PageFormModel, Page>(model, item);

                item.UpdatedDate = DateTime.Now;

                GetSession.Update(item);

                return RedirectToAction("Index", new { model.ParentId, model.SectionId });
            }
            return View(model);
        }

        [Admin, Transaction]
        public ActionResult Destroy(long id)
        {
            var item = GetSession.Get<Page>(id);

            if (item == null)
                return Content(string.Format("tried to delete page: {0} but it does not exists", id));

            long? parentId = (item.Parent == null ? (long?)null : item.Parent.Id);
            GetSession.Delete(item);
            return RedirectToAction("Index", new { parentId, item.SectionId });
        }

        [Admin, HttpPost, Transaction]
        public ContentResult PageOrder(long id, int pageOrder)
        {
            var item = GetSession.Get<Page>(id);
            item.PageOrder = pageOrder;
            GetSession.Update(item);
            return Content("ok");
        }

        [ChildActionOnly]
        public ActionResult Nav()
        {
            var items = GetSession.QueryOver<Page>()
                .Where(x => x.SectionId == 1 && x.Parent == null)
                .OrderBy(x => x.PageOrder).Asc
                .List()
                .ByPermissions(HttpContext);
            return PartialView(items);
        }


        [ChildActionOnly]
        public ActionResult LeftBoxesMaster()
        {
            var items = GetSession.Get<Setting>((long)1);
            return PartialView(items);
        }



        [ChildActionOnly]
        public ActionResult NavNews()
        {
            var items = GetSession.QueryOver<Page>()
                .Where(x => x.SectionId == 3 && x.Parent == null)
                .OrderBy(x => x.PageOrder).Asc
                .List()
                .ByPermissions(HttpContext);
            return PartialView(items);
        }


        public ActionResult RightMenu(Page currentPage)
        {
            Page pageToShow = GetFirstPage(currentPage);

            var items = GetSession.QueryOver<Page>()
                .Where(x => x.Parent.Id == pageToShow.Id)
                .OrderBy(x => x.PageOrder).Asc
                .List()
                .ByPermissions(HttpContext);
            return View(items);
        }


        [ChildActionOnly]
        public ActionResult FooterMenu()
        {
            var items = GetSession.QueryOver<Page>()
                .Where(x => x.SectionId == 3 && x.Parent == null && x.IsActive)
                .OrderBy(x => x.PageOrder).Asc
                .List()
                .ByPermissions(HttpContext);
            return PartialView(items);
        }


        public ActionResult Show(long? id)
        {

            if (id.HasValue)
            {
                var item = GetSession.QueryOver<Page>()
                    .Where(x => x.Id == id)
                    .List().FirstOrDefault();
                if (item.TypeId == 999)
                {
                    return View(item);
                }
            }



            if (id.HasValue)
            {
                var item = GetSession.QueryOver<Page>()
                    .Where(x => x.Id == id)
                    .List()
                    .ByPermissions(HttpContext)
                    .FirstOrDefault();



                if (item == null)
                    return RedirectToAction("NoAccess");

                return View(item);
            }
            return RedirectToAction("NoAccess");
        }

        [Admin]
        public ActionResult Permissions(long? id)
        {
            var item = GetSession.Get<Page>(id);

            var model = Mapper.Map<Page, PermissionsFormModel>(item);

            model.InitMembers(GetSession);

            return View(model);
        }

        [HttpPost, Admin, Transaction]
        public ActionResult Permissions(PermissionsFormModel model)
        {

            var item = GetSession.Get<Page>(model.Id);

            Mapper.Map<PermissionsFormModel, Page>(model, item);

            GetSession.Update(item);

            return RedirectToAction("Index", new { model.ParentId, model.SectionId });
        }

        public ActionResult Search(string query)
        {
            var items = GetSession.QueryOver<Page>()
                .Where(
                    (
                        Restrictions.On<Page>(x => x.Name).IsLike(query, MatchMode.Anywhere)
                        ||
                        Restrictions.On<Page>(x => x.Content).IsLike(query, MatchMode.Anywhere)
                    )
                )
                .OrderBy(x => x.Name).Asc
                .List()
                .ByPermissions(HttpContext);

        
            int sindex = 0;
            string first = "";
            string last = "";
            foreach (Page item in items)
            {
                if (!string.IsNullOrEmpty(item.Content))
                {
                    item.Content = Regex.Replace(item.Content, @"<[^>]+>|&nbsp;", "").Trim();
                    sindex = item.Content.IndexOf(query, 0);

                    sindex = (sindex - 100) < 0 ? 0 : sindex - 100;

                    first = item.Content.Substring(sindex, 100);
                    last = item.Content.Substring(sindex, 100);
                    item.Content = string.Concat(first, last);
                    
                }
                
                
            }




            var wikis = GetSession.QueryOver<Wiki>()
                .Where(
                    (
                        Restrictions.On<Wiki>(x => x.Title).IsLike(query, MatchMode.Anywhere)
                        ||
                        Restrictions.On<Wiki>(x => x.Content).IsLike(query, MatchMode.Anywhere)
                    )
                )
                .OrderBy(x => x.Title).Asc
                .List();




            var events = GetSession.QueryOver<Message>()
                        .Where(
                            (
                                Restrictions.On<Message>(x => x.Name).IsLike(query, MatchMode.Anywhere)
                                ||
                                Restrictions.On<Message>(x => x.Content).IsLike(query, MatchMode.Anywhere)
                            )
                        )
                            .OrderBy(x => x.Name).Asc
                            .List();

            var jobs = GetSession.QueryOver<Job>()
                                .Where(
                                    (
                                        Restrictions.On<Job>(x => x.Name).IsLike(query, MatchMode.Anywhere)
                                        ||
                                        Restrictions.On<Job>(x => x.Content).IsLike(query, MatchMode.Anywhere)
                                    )
                                )
                    .OrderBy(x => x.Name).Asc
                    .List();

            var calendars = GetSession.QueryOver<Calendar>()
                    .Where(
                        (
                            Restrictions.On<Calendar>(x => x.Abstract).IsLike(query, MatchMode.Anywhere)
                            ||
                            Restrictions.On<Calendar>(x => x.Body).IsLike(query, MatchMode.Anywhere)
                            ||
                            Restrictions.On<Calendar>(x => x.Title).IsLike(query, MatchMode.Anywhere)
                        )
                    )
                    .OrderBy(x => x.Id).Asc
                    .List();



            var galleries = GetSession.QueryOver<Gallery>()
                                .Where(
                                    (
                                        Restrictions.On<Gallery>(x => x.Title).IsLike(query, MatchMode.Anywhere)
                                    )
                                )
                    .OrderBy(x => x.Title).Asc
                    .List();


            var docs = GetSession.QueryOver<Doc>()
              .Where(
                  (
                      Restrictions.On<Doc>(x => x.Name).IsLike(query, MatchMode.Anywhere)
                      ||
                      Restrictions.On<Doc>(x => x.Content).IsLike(query, MatchMode.Anywhere)
                  )
              )
              .OrderBy(x => x.Id).Asc
              .List();

            long currEmp = GetEmployeeId;

            //IList<Doc> allowedDocs = docs.Where


            ViewBag.docs = docs;



            var forumsPosts = GetSession.QueryOver<Forum>()
                  .Where(
                      (
                          Restrictions.On<Forum>(x => x.Body).IsLike(query, MatchMode.Anywhere)
                          ||
                          Restrictions.On<Forum>(x => x.CategoryTitle).IsLike(query, MatchMode.Anywhere)
                      )
                  )
                  .OrderBy(x => x.Id).Asc
                  .List();




            ViewBag.events = events;
            ViewBag.jobs = jobs;
            ViewBag.calendars = calendars;
            ViewBag.galleries = galleries;
            
            ViewBag.forumsPosts = forumsPosts;

            ViewBag.Wiki = wikis;

            ViewBag.Query = query;

            return View(items);
        }

        public ActionResult NoAccess()
        {
            return View();
        }

        [Admin, Transaction, HttpPost]
        public ContentResult PageActive(long id, bool isActive)
        {
            var item = GetSession.Get<Page>(id);
            item.IsActive = isActive;
            GetSession.Update(item);
            return Content("ok");
        }

        public Page GetFirstPage(Page oPage)
        {
            if (oPage.Parent == null)
                return oPage;
            else
                return GetFirstPage(oPage.Parent);
        }



        public ActionResult Error_auth()
        {
            return View();
        }



        #region Import

        [Admin, Transaction, HttpPost, ValidateInput(false)]
        public JsonResult Import(Int64 id, PageImportModel model)
        {
            if (ModelState.IsValid)
            {
                string categoryTemplate = System.IO.File.ReadAllText(Server.MapPath("~" + HttpUtility.UrlDecode(model.TemplateUrl)));
                string categoryItemTemplate = System.IO.File.ReadAllText(Server.MapPath("~" + HttpUtility.UrlDecode(model.TemplateItemUrl)));
                string relativePath = HttpUtility.UrlDecode(model.FolderUrl);

                this.AddCategory(relativePath, 1, id, categoryTemplate, categoryItemTemplate, true);

                return Json("הייבוא התבצע בהצלחה");
            }

            return Json(null);
        }

        private void AddCategory(string relativePath, int sectionId, Int64 parentId,
            string categoryTemplate, string categoryItemTemplate, bool root = false)
        {
            string filePath = this.Server.MapPath("~" + relativePath);
            string[] subFolders = Directory.GetDirectories(filePath);

            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            Int64 pageId = parentId;

            // Ignore Root
            if (!root)
            {
                // Generate page
                //Page page = this.GetSession.QueryOver<Page>()
                //                    .Where(p => p.Id == parentId && p.Name == directoryInfo.Name)
                //                    .SingleOrDefault();

                //// Get page
                //if (page == null)
                //{
                Page page = new Page()
                {
                    SectionId = sectionId,
                    Name = directoryInfo.Name
                };
                //}

                page.Parent = GetSession.Get<Page>(parentId);

                FileInfo[] files = directoryInfo.GetFiles();
                if (files.Length > 0)
                {
                    // Generate Content
                    string content = (string)categoryTemplate.Clone();
                    StringBuilder sb = new StringBuilder();

                    foreach (FileInfo file in files)
                    {
                        sb.Append(categoryItemTemplate.Replace("%%FileUrl%%", (relativePath + "/" + file.Name).Replace("//", "/"))
                        .Replace("%%FileName%%", file.Name)
                        .Replace("%%FileDescription%%", String.Empty)
                        .Replace("%%FileDateTime%%", String.Empty));
                    }

                    content = categoryTemplate.Replace("%%Items%%", sb.ToString());
                    page.Content = content;

                    sb.Clear();
                }

                if (subFolders.Length > 1)
                {
                    page.TypeId = 1;
                }
                else
                {
                    page.TypeId = 2;
                }


                // Save Page
                AddCategoryPage(page);

                pageId = page.Id;
            }


            // Recursive sub folders
            foreach (string subFolder in subFolders)
            {
                directoryInfo = new DirectoryInfo(subFolder);

                AddCategory(relativePath + "/" + directoryInfo.Name, sectionId, pageId, categoryTemplate, categoryItemTemplate);
            }
        }

        private void AddCategoryPage(Page page)
        {
            int maxOrder = GetSession.Query<Page>()
                .Where(x => x.Parent.Id == page.Parent.Id && x.SectionId == page.SectionId)
                .Max(x => (int?)x.PageOrder) ?? 0;

            page.PageOrder = maxOrder + 1;
            page.IsInternal = true;
            page.UpdatedDate = DateTime.Now;

            GetSession.Save(page);

            //return RedirectToAction("Index", new { model.ParentId, model.SectionId });

        }

        #endregion
    }
}
