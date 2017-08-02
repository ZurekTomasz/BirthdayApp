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

        public void AddUser(string fName, string sName, string fEmail, string Password, string Role, string bDate)
        {
            try
            {
                var user = new ApplicationUser { UserName = fEmail, Email = fEmail };
                var result = UserManager.Create(user, Password);

                if (result.Succeeded)
                {
                    var mUser = new ModelUser(fName, sName, fEmail, Role, "1997-05-20", user.Id);
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
            AddUser("Tomasz", "Żurek", "mail@tomass.net", "123456", "Admin", "1997-05-20");
            AddUser("Aleksander", "Tabor", "aleksander@gmail.com", "123456", "User", "1996-02-10");
            AddUser("Jan", "Kowalski", "jan@gmail.com", "123456", "User", "1960-01-14"); 

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            AddUser("Eryk", "Nowakowski", "eryk@gmail.com", "123456", "User", "1997-05-20");

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

        public ActionResult AddCollectStatic()
        {
            ViewBag.Message = "AddCollectStatic";

            if (User.Identity.IsAuthenticated)
            {
                string userId = String.Empty;
                var userId_array = User.Identity.GetUserId();
                if (userId_array != null)
                {
                    userId = string.Join(String.Empty, userId_array.ToArray());
                    ViewBag.userid = userId;
                }


                //var newCollection = new Collect();
                //newCollection.UserId = userId;
                //newCollection.Name = "Zbiórka 1";

                //using (var context = new ApplicationDbContext())
                //{
                //    context.Collections.Add(newCollection);
                //    context.SaveChanges();

                //    int id = newCollection.Id;
                //    ViewBag.xid = id.ToString();
                //}
            }
            else
            {
                ViewBag.Message = "Użytkownik jest niezalogowany";
            }

            return View();
        }
        
    }
}