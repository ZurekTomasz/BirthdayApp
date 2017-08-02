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
    public class ModelUsersController : Controller
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

        // GET: ModelUsers
        [Authorize]
        public ActionResult Index()
        {
            //ModelUser modelUser = db.ModelUsers.Find(GetModelUserId());
            //modelUser.Surname = "Nazwisko";

            ModelUser modelUser = db.ModelUsers.Find(GetModelUserId());
            var collect = modelUser.Collects.First();
            //var collect = modelUser.Collects.Single(c => c.Id == 999);
            collect.Name = collect.Name + "_suffix";

            db.SaveChanges();



            return View(db.ModelUsers.ToList());
        }

        [Authorize]
        public ActionResult MyProfile()
        {
            var userId = User.Identity.GetUserId();

            var thisUser = (from i in db.ModelUsers
                            where i.EntityId == userId
                            select i).ToList();



            return View(thisUser);
        }

        public ActionResult ThisProfile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelUser modelUser = db.ModelUsers.Find(id);
            if (modelUser == null)
            {
                return HttpNotFound();
            }

            var oneUser = (from i in db.ModelUsers
                           where i.Id == id
                           select i).ToList();

            return View(oneUser);
        }

        // GET: ModelUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelUser modelUser = db.ModelUsers.Find(id);
            if (modelUser == null)
            {
                return HttpNotFound();
            }
            return View(modelUser);
        }

        // GET: ModelUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ModelUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EntityId,Firstname,Surname,Email,Role,DateOfBirth,DateOfAdd")] ModelUser modelUser)
        {
            if (ModelState.IsValid)
            {
                db.ModelUsers.Add(modelUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(modelUser);
        }

        [Authorize]
        public ActionResult EditMe()
        {
            ModelUser modelUser = db.ModelUsers.Find(GetModelUserId());

            if (modelUser == null)
            {
                return HttpNotFound();
            }
            return View(modelUser);
        }

        // GET: ModelUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ModelUser modelThisUser = db.ModelUsers.Find(GetModelUserId());
            ModelUser modelUser = db.ModelUsers.Find(id);
            
            if (modelUser == null)
            {
                return HttpNotFound();
            }

            if (modelThisUser.Role != "Admin")
            {
                return HttpNotFound();
            }
            return View(modelUser);
        }

        // POST: ModelUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EntityId,Firstname,Surname,Email,Role,DateOfBirth,DateOfAdd")] ModelUser modelUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(modelUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(modelUser);
        }

        // GET: ModelUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModelUser modelUser = db.ModelUsers.Find(id);
            if (modelUser == null)
            {
                return HttpNotFound();
            }
            return View(modelUser);
        }

        // POST: ModelUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModelUser modelUser = db.ModelUsers.Find(id);
            db.ModelUsers.Remove(modelUser);
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
