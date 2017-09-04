using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppModels;
using BirthdayApp.Models;
using Microsoft.AspNet.Identity;
using BirthdayApp.AppService;

namespace BirthdayApp.Controllers
{
    public class CollectUsersController : CommonController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int id)
        {
            int userID = GetUserId();
            if (!db.CollectionsUsers.Any(c => c.Collect.OwnerId == userID))
            {
                return HttpNotFound();
            }

            var collectionsUsers = db.CollectionsUsers.Include(c => c.Collect).Include(c => c.User);
            var collectionsUsers2 = collectionsUsers.Where(c => c.CollectId == id);
            if (collectionsUsers2 == null)
            {
                return HttpNotFound();
            }

            ViewBag.ThisId = id;

            return View(collectionsUsers2.ToList());
        }

        public ActionResult GaveMoney(int id)
        {

            CollectUser collectUser = db.CollectionsUsers.Find(id);

            int userID = GetUserId();
            if (!db.CollectionsUsers.Any(c => c.Collect.OwnerId == userID))
            {
                return HttpNotFound();
            }

            if (collectUser.GaveMoney)
            {
                collectUser.GaveMoney = false;
            }
            else
            {
                collectUser.GaveMoney = true;
            }

            db.CollectionsUsers.Attach(collectUser);
            db.Entry(collectUser).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "CollectUsers", new { id = collectUser.Collect.Id });
        }

        public ActionResult Join(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectViewModel = collectService.GetCollectViewModel(id, GetUserId());
                return View(collectViewModel);
            }
        }

        [HttpPost, ActionName("Join")]
        [ValidateAntiForgeryToken]
        public ActionResult JoinConfirmed(int id)
        {
            int userId = GetUserId();

            if (ModelState.IsValid)
            {
                if(!db.CollectionsUsers.Any(i => i.CollectId == id && i.UserId == userId))
                {
                    CollectUser collectUser = new CollectUser();
                    collectUser.UserId = userId;
                    collectUser.CollectId = id;
                    collectUser.GaveMoney = false;
                    db.CollectionsUsers.Add(collectUser);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Collects");
            }

            return HttpNotFound();
        }
        
        public ActionResult Leave(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectViewModel = collectService.GetCollectViewModel(id,GetUserId());
                return View(collectViewModel);
            }
        }

        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            int userId = GetUserId();
            int collectUsersID = db.CollectionsUsers.SingleOrDefault(i => i.CollectId == id && i.UserId == userId).Id;

            CollectUser collectUser = db.CollectionsUsers.Find(collectUsersID);
            db.CollectionsUsers.Remove(collectUser);
            db.SaveChanges();
            return RedirectToAction("Index", "Collects");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
