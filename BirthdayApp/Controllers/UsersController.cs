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
using BirthdayApp.AppService;

namespace BirthdayApp.Controllers
{
    public class UsersController : CommonController
    {
        public ActionResult Index()
        {
            using (var collectService = new CollectsService())
            {
                var users = collectService.GetUserIndex();
                return View(users);
            }
        }

        public ActionResult Details(int id)
        {
            using (var collectService = new CollectsService())
            {
                var user = collectService.GetUser(id);
                return View(user);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Firstname,Surname,EntityId,Email,Role,DateOfBirth,DateOfAdd")] User user)
        {
            if (ModelState.IsValid)
            {
                using (var collectService = new CollectsService())
                {
                    collectService.UserAdd(user);
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }

        public ActionResult Edit(int id)
        {
            using (var collectService = new CollectsService())
            {
                var user = collectService.GetUser(id);
                return View(user);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Firstname,Surname,EntityId,Email,Role,DateOfBirth,DateOfAdd")] User user)
        {
            if (ModelState.IsValid)
            {
                using (var collectService = new CollectsService())
                {
                    collectService.UserUpdate(user);
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //IsActive
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.MyUsers.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    User user = db.MyUsers.Find(id);
        //    db.MyUsers.Remove(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

    }
}
