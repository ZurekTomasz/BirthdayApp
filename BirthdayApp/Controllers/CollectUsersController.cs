﻿using System;
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
    public class CollectUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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