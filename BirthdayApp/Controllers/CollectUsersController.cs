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
    public class CollectUsersController : Controller
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

        // GET: CollectUsers
        public ActionResult Index()
        {
            var collectionsUsers = db.CollectionsUsers.Include(c => c.Collect).Include(c => c.User);
            return View(collectionsUsers.ToList());
        }

        // GET: CollectUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectUser collectUser = db.CollectionsUsers.Find(id);
            if (collectUser == null)
            {
                return HttpNotFound();
            }
            return View(collectUser);
        }

        // GET: CollectUsers/Create
        public ActionResult Create()
        {
            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name");
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name");
            return View();
        }

        // POST: CollectUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,CollectId,GaveMoney")] CollectUser collectUser)
        {
            if (ModelState.IsValid)
            {
                db.CollectionsUsers.Add(collectUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name", collectUser.CollectId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectUser.UserId);
            return View(collectUser);
        }

        // GET: CollectUsers/Create
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

            return RedirectToAction("CreateFID", "CollectUsers", new { id = id });
        }

        public ActionResult CreateFID(int? id)
        {
            if (ModelState.IsValid)
            {
                CollectUser collectUser = new CollectUser();
                collectUser.UserId = GetModelUserId();
                collectUser.CollectId = id;
                collectUser.GaveMoney = false;
                db.CollectionsUsers.Add(collectUser);
                db.SaveChanges();
                return RedirectToAction("Index", "Collects");
            }

            return HttpNotFound();
        }

        // GET: CollectUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectUser collectUser = db.CollectionsUsers.Find(id);
            if (collectUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name", collectUser.CollectId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectUser.UserId);
            return View(collectUser);
        }

        // POST: CollectUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,CollectId,GaveMoney")] CollectUser collectUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collectUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CollectId = new SelectList(db.Collections, "Id", "Name", collectUser.CollectId);
            ViewBag.UserId = new SelectList(db.ModelUsers, "Id", "Name", collectUser.UserId);
            return View(collectUser);
        }

        // GET: CollectUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollectUser collectUser = db.CollectionsUsers.Find(id);
            if (collectUser == null)
            {
                return HttpNotFound();
            }
            return View(collectUser);
        }

        // POST: CollectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CollectUser collectUser = db.CollectionsUsers.Find(id);
            db.CollectionsUsers.Remove(collectUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CollectUsers/Delete/5
        public ActionResult Delete2(int? id)
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

            int userID = GetModelUserId();
            int CollectUsersID = db.CollectionsUsers.SingleOrDefault(i => i.CollectId == id && i.UserId == userID).Id;

            return RedirectToAction("DeleteFID", "CollectUsers", new { id = CollectUsersID });
        }


        // POST: CollectUsers/Delete/5
        public ActionResult DeleteFID(int id)
        {
            CollectUser collectUser = db.CollectionsUsers.Find(id);
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
