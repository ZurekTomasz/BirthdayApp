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

namespace BirthdayApp.Controllers
{
    public class CollectGiftRatingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CollectGiftRatings
        public ActionResult Index()
        {
            var collectionsGiftRatings = db.CollectionsGiftRatings.Include(c => c.Gift).Include(c => c.User);
            return View(collectionsGiftRatings.ToList());
        }

        // GET: CollectGiftRatings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectGiftRating collectGiftRating = db.CollectionsGiftRatings.Find(id);
            if (collectGiftRating == null)
            {
                return HttpNotFound();
            }
            return View(collectGiftRating);
        }

        // GET: CollectGiftRatings/Create
        public ActionResult Create()
        {
            ViewBag.GiftId = new SelectList(db.CollectionsGifts, "Id", "Name");
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name");
            return View();
        }

        // POST: CollectGiftRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,GiftId,TheBestRating")] CollectGiftRating collectGiftRating)
        {
            if (ModelState.IsValid)
            {
                db.CollectionsGiftRatings.Add(collectGiftRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GiftId = new SelectList(db.CollectionsGifts, "Id", "Name", collectGiftRating.GiftId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectGiftRating.UserId);
            return View(collectGiftRating);
        }

        // GET: CollectGiftRatings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectGiftRating collectGiftRating = db.CollectionsGiftRatings.Find(id);
            if (collectGiftRating == null)
            {
                return HttpNotFound();
            }
            ViewBag.GiftId = new SelectList(db.CollectionsGifts, "Id", "Name", collectGiftRating.GiftId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectGiftRating.UserId);
            return View(collectGiftRating);
        }

        // POST: CollectGiftRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,GiftId,TheBestRating")] CollectGiftRating collectGiftRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collectGiftRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GiftId = new SelectList(db.CollectionsGifts, "Id", "Name", collectGiftRating.GiftId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectGiftRating.UserId);
            return View(collectGiftRating);
        }

        // GET: CollectGiftRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectGiftRating collectGiftRating = db.CollectionsGiftRatings.Find(id);
            if (collectGiftRating == null)
            {
                return HttpNotFound();
            }
            return View(collectGiftRating);
        }

        // POST: CollectGiftRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CollectGiftRating collectGiftRating = db.CollectionsGiftRatings.Find(id);
            db.CollectionsGiftRatings.Remove(collectGiftRating);
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
