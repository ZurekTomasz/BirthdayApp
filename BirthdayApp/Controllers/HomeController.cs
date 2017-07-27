using BirthdayApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BirthdayApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UsersList()
        {
            var userId = User.Identity.GetUserId();
            var context = new ApplicationDbContext();

            var allUsers = (from i in context.Users
                           select i).ToList();

            ViewBag.allUsersContext = allUsers;

            return View();
        }
    }
}