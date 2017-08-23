using BirthdayApp.DAL;
using BirthdayApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BirthdayApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer(new MyDbInitializer());

            //Database.SetInitializer(new DropCreateDatabaseAlways<ApplicationDbContext>());
            //using (ApplicationDbContext context = new ApplicationDbContext()) { context.Database.Delete(); }
        }

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    HttpContext.Current.ClearError();
        //    Server.ClearError();
        //    Response.Redirect("~/Home/ErrorPage/", false);
        //    return;
        //}

    }
}
