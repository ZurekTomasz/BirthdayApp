using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BirthdayApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using AppModels;

namespace BirthdayApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext(); 

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ErrorPage()
        {
            return View();
        }

        public ActionResult UsersList()
        {
            var context = new ApplicationDbContext();

            var allUsers = (from i in context.Users
                           select i).ToList();

            ViewBag.allUsersContext = allUsers;

            return View();
        }

        public string GetUserId()
        {
            string userId = User.Identity.GetUserId();
            return userId;
        }

        public int GetModelUserId()
        {
            string userId = User.Identity.GetUserId();
            int modelUserId = db.ModelUsers.Single(i => i.EntityId == userId).Id;

            return modelUserId;
        }

        public ActionResult AddCollectStatic()
        {
            ViewBag.Message = "AddCollectStatic";

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.com1 = "Użytkownik ID = " + GetUserId() + " dodał nową zbiórkę";

                var newCollection = new Collect();
                newCollection.DateOfInitiative = DateTime.ParseExact("2017-09-17", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                newCollection.OwnerId = GetModelUserId();
                newCollection.RecipientId = 2;
                newCollection.Name = "Zbiórka 1";
                //newCollection.Amount = 1.50m;

                db.Collections.Add(newCollection);
                db.SaveChanges();

                ViewBag.com2 = "Dodano zbiórkę nr: " + newCollection.Id;

            }
            else
            {
                ViewBag.Message = "Użytkownik jest niezalogowany";
            }

            return View();
        }
        
    }
}