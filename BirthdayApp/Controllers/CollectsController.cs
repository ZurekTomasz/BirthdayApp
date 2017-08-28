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
using Microsoft.AspNet.Identity;
using BirthdayApp.ViewModels;


namespace BirthdayApp.Controllers
{
    public class CollectsController : CommonController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        CollectsService collectService = new CollectsService();

        // GET: Collects
        public ActionResult Index()
        {
            int userId = GetModelUserId();
            var collections = collectService.AllCollectList(userId).Where(i => i.RecipientId != userId).OrderByDescending(i => i.DateOfAdd).ToList();

            return View(collections);
        }

        public ActionResult Details(int id)
        {
            var collectViewModel = collectService.UpdateCollectViewModel(id,GetModelUserId());

            return View(collectViewModel);
        }

        [HttpPost]
        public ActionResult Details(int id, CollectViewModel model)
        {
            int userId = GetModelUserId();

            if (ModelState.IsValid)
            {
                collectService.ChangeRadioButtonChoose(id, userId, model.Gift.Id);
            }

            var collectViewModel = collectService.UpdateCollectViewModel(id,GetModelUserId());

            return View(collectViewModel);
        }

        public ActionResult UndoConfirm(int id)
        {
            collectService.CollectConfirmChange(id, false);

            return RedirectToAction("Details2", "Collects", new { id = id });
        }

        // GET: Collects/Details/5
        public ActionResult Details2(int? id)
        {
            var collect = collectService.GetCollect(id.Value);
            int UserId = GetModelUserId();

            if(!IsAdmin())
            {
                if (!db.CollectionsUsers.Any(i => i.UserId == UserId && i.CollectId == id))
                {
                    return HttpNotFound();
                }
            }

            if(db.CollectionsGiftRatings.Any(i => i.TheBestRating == true & i.UserId == UserId))
            { 
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == UserId).GiftId;
            }
            else
            {
                ViewData["uniqueRadio"] = "0";
            }

            var collectrionsUsers = (from i in db.CollectionsUsers
                                     where i.CollectId == id
                                     select i).ToList();

            int gmcounter = 0;

            foreach (var item in collectrionsUsers)
            {
                if (item.GaveMoney)
                {
                    gmcounter++;
                }
            }

            ViewBag.gmcounter = gmcounter;


            foreach (var item in db.CollectionsGifts.OrderBy(i => i.Id).ToList())
            {
                item.Rating = 0;
                foreach (var item2 in db.CollectionsUsers.Where(i => i.CollectId == id).ToList())
                {
                    if(db.CollectionsGiftRatings.Any(i => i.GiftId == item.Id && i.UserId == item2.UserId && i.TheBestRating == true))
                    {
                        item.Rating++;
                    }
                }
            }

            ViewBag.UserId = GetModelUserId();

            int SelectedGiftId = 0;
            try
            {
                int? SelectedGiftIdNull = db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == db.Collections.FirstOrDefault(c => c.Id == id).OwnerId).GiftId;
                SelectedGiftId = SelectedGiftIdNull ?? 0;
            }
            catch { }

            string SelectedGiftName = "";
            if (SelectedGiftId != 0)
            {
                SelectedGiftName = db.CollectionsGifts.SingleOrDefault(i => i.Id == SelectedGiftId).Name;
            }
            
            ViewBag.SelectedGiftName = SelectedGiftName;

            decimal NumberUsersInCollect = db.CollectionsUsers.Count(c => c.CollectId == id);
            decimal MyAmount = db.Collections.SingleOrDefault(c => c.Id == id).Amount;
            decimal AmountPerPerson = MyAmount / NumberUsersInCollect;

            ViewBag.AmountPerPerson = AmountPerPerson;

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

            var collectrionsUsers = (from i in db.CollectionsUsers
                                     where i.CollectId == id
                                     select i).ToList();

            int gmcounter = 0;

            foreach (var item in collectrionsUsers)
            {
                if (item.GaveMoney)
                {
                    gmcounter++;
                }
            }

            ViewBag.gmcounter = gmcounter;

            foreach (var item in db.CollectionsGifts.OrderBy(i => i.Id).ToList())
            {
                item.Rating = 0;
                foreach (var item2 in db.CollectionsUsers.Where(i => i.CollectId == id).ToList())
                {
                    if (db.CollectionsGiftRatings.Any(i => i.GiftId == item.Id && i.UserId == item2.UserId && i.TheBestRating == true))
                    {
                        item.Rating++;
                    }
                }
            }

            ViewBag.UserId = GetModelUserId();

            return View(collect);
        }

        // GET: Collects/Details/5
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
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.Single(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;
                //ViewData["uniqueRadio"] = db.CollectionsGiftRatings.SingleOrDefault(i => i.TheBestRating == true & i.UserId == db.Collections.FirstOrDefault(c => c.Id == id).OwnerId).GiftId;
            }
            else
            {
                ViewData["uniqueRadio"] = "0";
            }

            foreach (var item in db.CollectionsGifts.OrderBy(i => i.Id).ToList())
            {
                item.Rating = 0;
                foreach (var item2 in db.CollectionsUsers.Where(i => i.CollectId == id).ToList())
                {
                    if (db.CollectionsGiftRatings.Any(i => i.GiftId == item.Id && i.UserId == item2.UserId && i.TheBestRating == true))
                    {
                        item.Rating++;
                    }
                }
            }

            if (ViewData["uniqueRadio"].ToString() == "0")
            {
                ViewBag.MustChooseItem = "Musisz wybrać któryś z prezentów !";
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

            ViewBag.userid = GetModelUserId();

            int wybranyid = 0;
            try
            {
                int? wybranyidNull = Int32.Parse(Request.Form["uniqueRadio"]);
                wybranyid = wybranyidNull ?? 0;
            }
            catch { }

            //int ggrid = db.CollectionsGiftRatings.SingleOrDefault(i => i.Id == wybranyid).Id;

            if (Request.Form["uniqueRadio"] == "0")
            {
                wybranyid = 0;
            }



            var ModelUserId = GetModelUserId();


            if (wybranyid != 0)
            {

            





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

            try
            {
                ViewData["uniqueRadio"] = db.CollectionsGiftRatings.Single(i => i.TheBestRating == true & i.UserId == ModelUserId).GiftId;
            }
            catch (Exception)
            {
                ViewData["uniqueRadio"] = "0";
            }

            int mojakwota = 0;
            try
            {
                int? mojakwotaNull = Int32.Parse(Request.Form["fname"]);
                mojakwota = mojakwotaNull ?? 0;
            }
            catch { }

            //ViewBag.mojakwota = mojakwota;
            //int liczba_uzytkownikow_w_zrzutce = db.CollectionsUsers.Count(c => c.CollectId == id);
            //int nalepka = Int32.Parse(mojakwota) / liczba_uzytkownikow_w_zrzutce;
            //ViewBag.nalepka = nalepka;
            //ViewBag.liczba_uzytkownikow_w_zrzutce = liczba_uzytkownikow_w_zrzutce;

            collect.Amount = mojakwota;
            collect.IsConfirmed = true;
            db.Collections.Attach(collect);
            db.Entry(collect).State = EntityState.Modified;
            db.SaveChanges();

            foreach (var item in db.CollectionsGifts.OrderBy(i => i.Id).ToList())
            {
                item.Rating = 0;
                foreach (var item2 in db.CollectionsUsers.Where(i => i.CollectId == id).ToList())
                {
                    if (db.CollectionsGiftRatings.Any(i => i.GiftId == item.Id && i.UserId == item2.UserId && i.TheBestRating == true))
                    {
                        item.Rating++;
                    }
                }
            }
            }
            else
            {
                ViewBag.MustChooseItem = "Musisz wybrać któryś z prezentów !";
            }


            if (wybranyid == 0)
            {
                return RedirectToAction("Confirm", "Collects", new { id = id });
            }

            return RedirectToAction("Details2","Collects", new { id = id });

        }

        // GET: Collects/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.MyUsers, "Id", "Name");
            ViewBag.RecipientId = new SelectList(db.MyUsers, "Id", "Name");
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

                //Add user to collect
                var newCollectionUsers = new CollectUser();
                newCollectionUsers.UserId = collect.OwnerId;
                newCollectionUsers.CollectId = collect.Id;
                db.CollectionsUsers.Add(newCollectionUsers);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.MyUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.MyUsers, "Id", "Name", collect.RecipientId);
            return View(collect);
        }

        // GET: Collects/Create
        public ActionResult Create2()
        {
            int UserId = GetModelUserId();
            var ModelUsersWithoutThisUser = db.MyUsers.Where(i => i.Id != UserId);
            ViewBag.RecipientId = new SelectList(ModelUsersWithoutThisUser, "Id", "Name");
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

                //Add user to collect
                var newCollectionUsers = new CollectUser();
                newCollectionUsers.UserId = collect.OwnerId;
                newCollectionUsers.CollectId = collect.Id;
                db.CollectionsUsers.Add(newCollectionUsers);
                db.SaveChanges();

                return RedirectToAction("Index", "CollectUsersListBox", new { id = collect.Id });
            }

            ViewBag.OwnerId = new SelectList(db.MyUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.MyUsers, "Id", "Name", collect.RecipientId);
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
            ViewBag.OwnerId = new SelectList(db.MyUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.MyUsers, "Id", "Name", collect.RecipientId);
            return View(collect);
        }

        // POST: Collects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,RecipientId,Name,Description,Amount,DateOfInitiative")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collect).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.MyUsers, "Id", "Name", collect.OwnerId);
            ViewBag.RecipientId = new SelectList(db.MyUsers, "Id", "Name", collect.RecipientId);
            return View(collect);
        }

        // GET: Collects/Edit/5
        public ActionResult Edit2(int? id)
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

        // POST: Collects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2([Bind(Include = "Id,OwnerId,RecipientId,Name,Description,Amount,DateOfInitiative")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collect).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
