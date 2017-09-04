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
using BirthdayApp.AppService;

namespace BirthdayApp.Controllers
{
    public class CollectGiftsController : CommonController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int id)
        {
            using (var collectService = new CollectsService())
            {
                ViewBag.collectId = id;
                var collectionsGifts = collectService.GetCollectsGiftsIndex(id);
                return View(collectionsGifts);
            }
        }
        
        public ActionResult Create(int id)
        {
                ViewBag.collectId = id;

                return View();      
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? id, [Bind(Include = "Id,Name")] CollectGift collectGift)
        {
            using (var collectService = new CollectsService())
            {
                if (ModelState.IsValid)
                {
                    collectGift.UserId = GetUserId();
                    collectGift.CollectId = id;
                    collectService.CollectGiftAdd(collectGift);

                    return RedirectToAction("Details", "Collects", new { id = id });
                }

                return View(collectGift);
            }
        }
        
        public ActionResult Delete(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectGift = collectService.GetCollectGift(id);
                return View(collectGift);
            } 
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var collectService = new CollectsService())
            {
                collectService.CollectGiftDelete(id);
                return RedirectToAction("Index");
            }
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
