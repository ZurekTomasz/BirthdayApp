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

        public bool IsAdmin()
        {
            int userID = GetModelUserId();
            if ("Admin" == db.ModelUsers.SingleOrDefault(i => i.Id == userID).Role)
            {
                return true;
            }

            return false;
        }

        // GET: Collects
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.id = GetModelUserId();
            var collections = db.Collections.Include(c => c.Owner).Include(c => c.Recipient);
            return View(collections.ToList());
        }

        // GET: Collects/Details/5
        [Authorize]
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
        [Authorize]
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
            int userID = GetModelUserId();


            if(!IsAdmin())
            {
                if (!db.CollectionsUsers.Any(i => i.UserId == userID && i.CollectId == id))
                {
                    return HttpNotFound();
                }
            }

            var ModelUserId = GetModelUserId();
            if(db.CollectionsGiftRatings.Any(i => i.TheBestRating == true & i.UserId == ModelUserId))
            { 
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;
            }
            else
            {
                ViewData["uniqueRadio"] = "0";
            }


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

            if (Request.Form["uniqueRadio"] == "0")
            {
                wybranyid = 0;
            }

            var ModelUserId = GetModelUserId();
            if(db.CollectionsGiftRatings.Any(i => i.TheBestRating == true & i.UserId == ModelUserId))
            {
                CollectGiftRating cgrx = db.CollectionsGiftRatings.Find(db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == ModelUserId).Id);
                cgrx.TheBestRating = false;
                db.SaveChanges();
            }
                

            //////////////**//
            if(db.CollectionsGiftRatings.Any(i => i.GiftId == wybranyid & i.UserId == ModelUserId))
            {
                CollectGiftRating cgr = db.CollectionsGiftRatings.Find(db.CollectionsGiftRatings.SingleOrDefault(i => i.GiftId == wybranyid & i.UserId == ModelUserId).Id);
                cgr.TheBestRating = true;
                db.SaveChanges();
            }
            else
            {
                if(wybranyid!=0)
                {
                    var cgr = new CollectGiftRating();
                    cgr.UserId = ModelUserId;
                    cgr.GiftId = wybranyid;
                    cgr.TheBestRating = true;
                    db.CollectionsGiftRatings.Add(cgr);
                    db.SaveChanges();
                }
            }


            

            var YourRadioButtonx = Request.Form["uniqueRadio"];
            ViewBag.wybor = wybranyid;
            ViewBag.userid = GetModelUserId();

            try
            {
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.Single(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;
            }
            catch (Exception)
            {
                ViewData["uniqueRadio"] = "0";
            }
   
            return View(collect);
        }

        // GET: Collects/Details/5
        [Authorize]
        public ActionResult Confirm(int? id)
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


            if (!IsAdmin())
            {
                if (!db.CollectionsUsers.Any(i => i.UserId == userID && i.CollectId == id))
                {
                    return HttpNotFound();
                }
            }

            var ModelUserId = GetModelUserId();
            if (db.CollectionsGiftRatings.Any(i => i.TheBestRating == true & i.UserId == ModelUserId))
            {
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;
            }
            else
            {
                ViewData["uniqueRadio"] = "0";
            }


            return View(collect);
        }



        [HttpPost]
        public ActionResult Confirm(int? id, string uniqueRadio, string fname)
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

            if (Request.Form["uniqueRadio"] == "0")
            {
                wybranyid = 0;
            }

            var ModelUserId = GetModelUserId();
            if (db.CollectionsGiftRatings.Any(i => i.TheBestRating == true & i.UserId == ModelUserId))
            {
                CollectGiftRating cgrx = db.CollectionsGiftRatings.Find(db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == ModelUserId).Id);
                cgrx.TheBestRating = false;
                db.SaveChanges();
            }


            //////////////**//
            if (db.CollectionsGiftRatings.Any(i => i.GiftId == wybranyid & i.UserId == ModelUserId))
            {
                CollectGiftRating cgr = db.CollectionsGiftRatings.Find(db.CollectionsGiftRatings.SingleOrDefault(i => i.GiftId == wybranyid & i.UserId == ModelUserId).Id);
                cgr.TheBestRating = true;
                db.SaveChanges();
            }
            else
            {
                if (wybranyid != 0)
                {
                    var cgr = new CollectGiftRating();
                    cgr.UserId = ModelUserId;
                    cgr.GiftId = wybranyid;
                    cgr.TheBestRating = true;
                    db.CollectionsGiftRatings.Add(cgr);
                    db.SaveChanges();
                }
            }




            var YourRadioButtonx = Request.Form["uniqueRadio"];
            ViewBag.wybor = wybranyid;
            ViewBag.userid = GetModelUserId();

            try
            {
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.Single(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;
            }
            catch (Exception)
            {
                ViewData["uniqueRadio"] = "0";
            }

            string mojakwota = Request.Form["fname"];
            ViewBag.mojakwota = mojakwota;
            int liczba_uzytkownikow_w_zrzutce = db.CollectionsUsers.Count(c => c.CollectId == id);
            int nalepka = Int32.Parse(mojakwota) / liczba_uzytkownikow_w_zrzutce;
            ViewBag.nalepka = nalepka;
            ViewBag.liczba_uzytkownikow_w_zrzutce = liczba_uzytkownikow_w_zrzutce;

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
        public ActionResult Create([Bind(Include = "Id,OwnerId,RecipientId,Name,Description,Amount,DateOfInitiative")] Collect collect)
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

        // GET: Collects/Create
        public ActionResult Create2()
        {
            ViewBag.RecipientId = new SelectList(db.ModelUsers, "Id", "Name");
            return View();
        }

        // POST: Collects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2([Bind(Include = "Id,RecipientId,Name,Description,Amount,DateOfInitiative")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                collect.OwnerId = GetModelUserId();
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
            var collectGift = db.CollectionsGifts.Where(x => x.CollectId == collect.Id);
            foreach (var item in collectGift.ToList())
            {
                var collectGiftRating = db.CollectionsGiftRatings.Where(x => x.GiftId == item.Id);
                db.CollectionsGiftRatings.RemoveRange(collectGiftRating);
                db.SaveChanges();
            }
            var collectUser = db.CollectionsUsers.Where(x => x.CollectId == collect.Id);
            db.CollectionsGifts.RemoveRange(collectGift);
            db.CollectionsUsers.RemoveRange(collectUser);
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
