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

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public void AddUser(string fName, string sName, string fEmail, string Password, string Role, DateTime? bDate)
        {
            try
            {
                var user = new ApplicationUser { UserName = fEmail, Email = fEmail };
                var result = UserManager.Create(user, Password);

                if (result.Succeeded)
                {
                    var mUser = new ModelUser(fName, sName, fEmail, Role, bDate, user.Id);
                    db.ModelUsers.Add(mUser);
                    db.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                ViewBag.Ex = Ex;
            }
        }

        public ActionResult Index()
        {
            var date1 = DateTime.ParseExact("1997-05-20", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            AddUser("Tomasz", "Żurek", "mail@tomass.net", "123456", "Admin", date1);
            AddUser("Aleksander", "Tabor", "aleksander@gmail.com", "123456", "User", date1);
            AddUser("Jan", "Kowalski", "jan@gmail.com", "123456", "User", date1); 

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            var date2 = DateTime.ParseExact("1990-08-24", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            AddUser("Eryk", "Nowakowski", "eryk@gmail.com", "123456", "User", date2);

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            

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
                newCollection.OwnerId = GetModelUserId();
                newCollection.RecipientId = 3;
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