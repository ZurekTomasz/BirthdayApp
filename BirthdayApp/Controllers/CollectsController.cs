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
            if (ModelState.IsValid)
            {
                collectService.ChangeRadioButtonChoose(id, GetModelUserId(), model.Gift.Id);
            }

            var collectViewModel = collectService.UpdateCollectViewModel(id,GetModelUserId());

            return View(collectViewModel);
        }

        public ActionResult UndoConfirm(int id)
        {
            collectService.CollectConfirmChange(id, false);

            return RedirectToAction("Details", "Collects", new { id = id });
        }

        public ActionResult Confirm(int id)
        {
            var collectViewModel = collectService.UpdateCollectViewModel(id, GetModelUserId());

            if (!IsAdmin())
            {
                if (!collectService.IsCollectionsUser(id, GetModelUserId()))
                {
                    return HttpNotFound();
                }
            }

            if(collectService.CounterCollectionsGifts(id) == 0)
                ViewBag.MustChooseItem = "Musisz wybrać któryś z prezentów !";

            return View(collectViewModel);
        }


        [HttpPost]
        public ActionResult Confirm(int id, string uniqueRadio, string fname, CollectViewModel model)
        {
            try
            {
                int selectedId = Int32.Parse(model.Gift.Id);
                if (ModelState.IsValid)
                {
                    collectService.ChangeRadioButtonChoose(id, GetModelUserId(), model.Gift.Id);
                }
                collectService.CollectAmountChange(id, model.Amount);
                collectService.CollectConfirmChange(id, true);
                return RedirectToAction("Details", "Collects", new { id = id });
            }
            catch(NullReferenceException)
            {
                ViewBag.MustChooseItem = "Musisz wybrać któryś z prezentów !";
            }

            var collectViewModel = collectService.UpdateCollectViewModel(id, GetModelUserId());

            return View(collectViewModel);
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
