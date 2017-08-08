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
    public class CollectsController : Controller
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

        // GET: Collects
        public ActionResult Index()
        {
            var collections = db.Collections.Include(c => c.Owner).Include(c => c.Recipient);
            return View(collections.ToList());
        }

        // GET: Collects/Details/5
        public ActionResult Details(int? id)
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
            return View(collect);
        }

        // GET: Collects/Details/5
        public ActionResult Details2(int? id)
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

            var ModelUserId = GetModelUserId();
            ViewData["uniqueRadio"] = db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;

            return View(collect);
        }

        

        [HttpPost]
        public ActionResult Details2(int? id, string uniqueRadio)
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

            


            int wybranyid = Int32.Parse(Request.Form["uniqueRadio"]);
            //int ggrid = db.CollectionsGiftRatings.SingleOrDefault(i => i.Id == wybranyid).Id;
            var ModelUserId = GetModelUserId();
            CollectGiftRating cgrx = db.CollectionsGiftRatings.Find(db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId);
            cgrx.TheBestRating = false;
            db.SaveChanges();

            CollectGiftRating cgr = db.CollectionsGiftRatings.Find(wybranyid);
            cgr.TheBestRating = true;
            db.SaveChanges();

            var YourRadioButtonx = Request.Form["uniqueRadio"];
            ViewBag.wybor = wybranyid;
            ViewBag.userid = GetModelUserId();

            
            ViewData["uniqueRadio"] = db.CollectionsGiftRatings.Single(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;

            return View(collect);
        }

        // GET: Collects/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.ModelUsers, "Id", "Name");
            ViewBag.RecipientId = new SelectList(db.ModelUsers, "Id", "Name");
            return View();
        }

        // POST: Collects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerId,RecipientId,Name,Description,Amount")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                db.Collections.Add(collect);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.ModelUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.ModelUsers, "Id", "Name", collect.RecipientId);
            return View(collect);
        }

        // GET: Collects/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.OwnerId = new SelectList(db.ModelUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.ModelUsers, "Id", "Name", collect.RecipientId);
            return View(collect);
        }

        // POST: Collects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,RecipientId,Name,Description,Amount")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collect).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.ModelUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.ModelUsers, "Id", "Name", collect.RecipientId);
            return View(collect);
        }

        // GET: Collects/Delete/5
        public ActionResult Delete(int? id)
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
            return View(collect);
        }

        // POST: Collects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Collect collect = db.Collections.Find(id);
            db.Collections.Remove(collect);
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
