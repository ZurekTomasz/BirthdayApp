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

namespace BirthdayApp.Controllers
{
    public class CollectGiftsController : CommonController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var collectionsGifts = db.CollectionsGifts.Include(c => c.Collect).Include(c => c.User);
            var collectionsGifts2 = collectionsGifts.Where(c => c.CollectId == id);
            if (collectionsGifts2 == null)
            {
                return HttpNotFound();
            }

            ViewBag.ThisId = id;

            return View(collectionsGifts2.ToList());
        }
        
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collect collect = db.Collections.Find(id);
            if (collect == null)
            {
                return HttpNotFound();
            }

            ViewBag.CollectId = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id, [Bind(Include = "Id,Name")] CollectGift collectGift)
        {
            if (ModelState.IsValid)
            {
                collectGift.UserId = GetUserId();
                collectGift.CollectId = id;
                db.CollectionsGifts.Add(collectGift);
                db.SaveChanges();
                return RedirectToAction("Details", "Collects", new { id = id });
            }

            return View(collectGift);
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectGift collectGift = db.CollectionsGifts.Find(id);
            if (collectGift == null)
            {
                return HttpNotFound();
            }
            return View(collectGift);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CollectGift collectGift = db.CollectionsGifts.Find(id);
            db.CollectionsGifts.Remove(collectGift);
            db.SaveChanges();
            return RedirectToAction("Index");
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
