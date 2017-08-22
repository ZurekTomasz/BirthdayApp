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
    public class CollectGiftsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

        // GET: CollectGifts
        public ActionResult Index()
        {
            var collectionsGifts = db.CollectionsGifts.Include(c => c.Collect).Include(c => c.User);
            return View(collectionsGifts.ToList());
        }

        // GET: CollectGifts
        public ActionResult IndexById(int? id)
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

        // GET: CollectGifts/Details/5
        public ActionResult Details(int? id)
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

        // GET: CollectGifts/Create
        public ActionResult Create()
        {
            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name");
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name");
            return View();
        }

        // POST: CollectGifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,UserId,CollectId,Rating")] CollectGift collectGift)
        {
            if (ModelState.IsValid)
            {
                db.CollectionsGifts.Add(collectGift);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name", collectGift.CollectId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectGift.UserId);
            return View(collectGift);
        }

        // GET: CollectGifts/Create
        public ActionResult Create2(int? id)
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

        // POST: CollectGifts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(int? id, [Bind(Include = "Id,Name,Description")] CollectGift collectGift)
        {
            if (ModelState.IsValid)
            {
                collectGift.UserId = GetModelUserId();
                collectGift.CollectId = id;
                collectGift.Rating = 0;
                db.CollectionsGifts.Add(collectGift);
                db.SaveChanges();
                return RedirectToAction("Details2", "Collects", new { id = id });
            }

            return View(collectGift);
        }

        // GET: CollectGifts/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name", collectGift.CollectId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectGift.UserId);
            return View(collectGift);
        }

        // POST: CollectGifts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,UserId,CollectId,Rating")] CollectGift collectGift)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collectGift).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name", collectGift.CollectId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectGift.UserId);
            return View(collectGift);
        }

        // GET: CollectGifts/Delete/5
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

        // POST: CollectGifts/Delete/5
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
