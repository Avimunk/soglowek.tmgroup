using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using AutoMapper;
using Portal.Entities;
using System.IO;
using FluentValidation.Mvc;
using FluentValidation.Attributes;
using System.Web.Helpers;
using Portal.Helpers;
using Portal.Models.Pages;
using Portal.Models.Polls;
using Portal.Attributes;
using Portal.Models.Messages;
using Portal.Models.StreamLink;
using Portal.Models.Jobs;
using Portal.Binders;
using Portal.Controllers;
using Portal.Models.Forums;
using Portal.Models.Calendars;
using Portal.Models.Galleries;
using Portal.Models.Employees;
using Portal.Models.JobTitles;
using Portal.Models.Departments;
using Portal.Models.Wikis;
using Portal.Models.Docs;
using Portal.Models.FlashBanners;
using Elmah;



namespace Portal
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new AuthorizeAttribute());
            //filters.Add(new EmployeeAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                    "Default_Admin", // Route name
                    "Admin", // URL with parameters
                    new { controller = "Pages", action = "Index" } // Parameter defaults
                );

            routes.MapRoute(
                "Login", // Route name
                "login", // URL with parameters
                new { controller = "Misc", action = "Login" } // Parameter defaults
            );

            routes.MapRoute(
"forumsAuto", // Route name
"Forums/AutoCreate", // URL with parameters
new { controller = "Forums", action = "AutoCreate"} // Parameter defaults
);

            routes.MapRoute(
"forums", // Route name
"Forums/{categoryId}", // URL with parameters
new { controller = "Forums", action = "Index", categoryId = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
"forum_list", // Route name
"Forums/List/{categoryId}", // URL with parameters
new { controller = "Forums", action = "List", categoryId = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
               "forums_create", // Route name
               "Forums/{categoryId}/post/{parentId}", // URL with parameters
               new { controller = "Forums", action = "Create", parentId = UrlParameter.Optional } // Parameter defaults
           );

            routes.MapRoute(
"forum_title", // Route name
"Forums/ForumTitle/{categoryId}/{title}", // URL with parameters
new { controller = "Forums", action = "ForumTitle", categoryId = UrlParameter.Optional, title = UrlParameter.Optional } // Parameter defaults
);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );



        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ConfigAutoMapper();

            InitializeValidator();
            InitializeData();

            System.Web.Mvc.ModelBinders.Binders.DefaultBinder = new TrimModelBinder();

#if DEBUG
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
#endif
        }

        private void InitializeData()
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var t = session.BeginTransaction())
            {
                var item = session.Get<Setting>((long)1);

                if (item == null)
                {
                    session.Save(new Setting());
                }

                t.Commit();
            }
        }

        static bool is_created = false;
        protected void Application_BeginRequest()
        {
            if (!is_created)
            {
                CreateFolders();
                is_created = true;
            }
        }

        private void CreateFolders()
        {
            string[] folders = new[] { "home", "pages", "galleries", "logs", "ckfinder", "employees", "flashBanners", "gallery", "employeephotos/small", "employeephotos/big", "docs" };

            foreach (string item in folders)
            {
                string check = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "public/userfiles", item);
                if (!Directory.Exists(check))
                {
                    try
                    {
                        Directory.CreateDirectory(check);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.Write("cannot create: " + check + " (" + ex.GetBaseException().Message + ")");
                        HttpContext.Current.Response.End();
                    }
                }
            }
        }

        public static string Config(string key, string def = "")
        {
            return ConfigurationManager.AppSettings[key] ?? def;
        }

        private void ConfigAutoMapper()
        {
            Mapper.CreateMap<PageFormModel, Page>();
            Mapper.CreateMap<Page, PageFormModel>();

            Mapper.CreateMap<PermissionsFormModel, Page>()
                .ForMember(x => x.Name, opt => opt.Ignore())

                .ForMember(x => x.Department, opt => opt.MapFrom(x =>
                    (x.DepartmentId == null ? "" : string.Join(",", x.DepartmentId))
                ))
                .ForMember(x => x.JobTitle, opt => opt.MapFrom(x =>
                    (x.JobTitleId == null ? "" : string.Join(",", x.JobTitleId))
                ))
                .ForMember(x => x.Employee, opt => opt.MapFrom(x =>
                    (x.EmployeeId == null ? "" : string.Join(",", x.EmployeeId))
                ))


            ;
            Mapper.CreateMap<Page, PermissionsFormModel>();

            Mapper.CreateMap<PollFormModel, Poll>();
            Mapper.CreateMap<Poll, PollFormModel>();

            Mapper.CreateMap<PollItemFormModel, PollItem>();
            Mapper.CreateMap<PollItem, PollItemFormModel>();

            Mapper.CreateMap<MessageFormModel, Message>();
            Mapper.CreateMap<Message, MessageFormModel>();


            Mapper.CreateMap<JobFormModel, Job>();
            Mapper.CreateMap<Job, JobFormModel>();

            Mapper.CreateMap<ForumFormModel, Forum>();
            Mapper.CreateMap<Forum, ForumFormModel>();

            Mapper.CreateMap<ForumUsersModel, ForumUsers>();
            Mapper.CreateMap<ForumUsers, ForumUsersModel>();


            Mapper.CreateMap<ForumItemModel, Forum>();
            Mapper.CreateMap<Forum, ForumItemModel>();

            Mapper.CreateMap<StreamLinkFormModel, StreamLink>();
            Mapper.CreateMap<StreamLink, StreamLinkFormModel>();

            Mapper.CreateMap<CalendarFormModel, Calendar>();
            Mapper.CreateMap<Calendar, CalendarFormModel>();

            Mapper.CreateMap<GalleryFormModel, Gallery>();
            Mapper.CreateMap<Gallery, GalleryFormModel>();


            Mapper.CreateMap<EmployeeFormModel, Employee>();
            Mapper.CreateMap<Employee, EmployeeFormModel>();

            Mapper.CreateMap<JobTitleFormModel, JobTitle>();
            Mapper.CreateMap<JobTitle, JobTitleFormModel>();

            Mapper.CreateMap<DepartmentFormModel, Department>();
            Mapper.CreateMap<Department, DepartmentFormModel>();

            Mapper.CreateMap<WikiFormModel, Wiki>();
            Mapper.CreateMap<Wiki, WikiFormModel>();

            Mapper.CreateMap<DocFormModel, Doc>();
            Mapper.CreateMap<Doc, DocFormModel>();

            Mapper.CreateMap<FlashBannerFormModel, FlashBanner>();
            Mapper.CreateMap<FlashBanner, FlashBannerFormModel>();

        }

        private static void InitializeValidator()
        {
            ModelMetadataProviders.Current = new DataAnnotationsModelMetadataProvider();
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new AttributedValidatorFactory()));

        }








    }

}