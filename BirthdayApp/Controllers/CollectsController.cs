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
        public ActionResult Index()
        {
            int userId = GetUserId();
            using (var collectService = new CollectsService())
            {
                var collections = collectService.AllCollectList(userId).Where(i => i.RecipientId != userId).OrderByDescending(i => i.DateOfAdd).ToList();
                return View(collections);
            }
        }

        public ActionResult Details(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectViewModel = collectService.GetCollectViewModel(id, GetUserId());
                return View(collectViewModel);
            }
        }

        [HttpPost]
        public ActionResult Details(int id, CollectViewModel model)
        {
            using (var collectService = new CollectsService())
            {
                if (ModelState.IsValid)
                    collectService.ChangeRadioButtonChoose(id, GetUserId(), model.Gift.Id);

                var collectViewModel = collectService.GetCollectViewModel(id, GetUserId());
                return View(collectViewModel);
            }
        }

        public ActionResult UndoConfirm(int id)
        {
            using (var collectService = new CollectsService())
            {
                collectService.CollectConfirmChange(id, false);
            }

            return RedirectToAction("Details", "Collects", new { id = id });
        }

        public ActionResult Confirm(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectViewModel = collectService.GetCollectViewModel(id, GetUserId());

                if (!collectService.IsAdmin(GetUserId()))
                {
                    if (!collectService.IsCollectionsUser(id, GetUserId()))
                    {
                        return HttpNotFound();
                    }
                }

                if (collectService.CounterCollectionsGifts(id) == 0)
                    ViewBag.MustChooseItem = "Musisz wybrać któryś z prezentów !";

                return View(collectViewModel);
            }
        }


        [HttpPost]
        public ActionResult Confirm(int id, string uniqueRadio, string fname, CollectViewModel model)
        {
            using (var collectService = new CollectsService())
            {
                try
                {
                    int selectedId = Int32.Parse(model.Gift.Id);
                    if (ModelState.IsValid)
                    {
                        collectService.ChangeRadioButtonChoose(id, GetUserId(), model.Gift.Id);
                    }
                    collectService.CollectAmountChange(id, model.Amount);
                    collectService.CollectConfirmChange(id, true);
                    return RedirectToAction("Details", "Collects", new { id = id });
                }
                catch (NullReferenceException)
                {
                    ViewBag.MustChooseItem = "Musisz wybrać jakikolwiek prezent !";
                }

                var collectViewModel = collectService.GetCollectViewModel(id, GetUserId());

                return View(collectViewModel);
            }
        }

        public ActionResult Create()
        {
            using (var collectService = new CollectsService())
            {
                int userId = GetUserId();
                var modelUsersWithoutThisUser = collectService.AllMyUserList().Where(i => i.Id != userId);
                ViewBag.RecipientId = new SelectList(modelUsersWithoutThisUser, "Id", "Name");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RecipientId,Name,Description,Amount,DateOfInitiative")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                using (var collectService = new CollectsService())
                {
                    collect.OwnerId = GetUserId();
                    collectService.CollectAdd(collect);

                    var newCollectionUsers = new CollectUser();
                    newCollectionUsers.UserId = collect.OwnerId;
                    newCollectionUsers.CollectId = collect.Id;
                    collectService.CollectUserAdd(newCollectionUsers);

                    return RedirectToAction("Index", "CollectUsersListBox", new { id = collect.Id });
                }
            }

            using (var collectService = new CollectsService())
            {
                ViewBag.OwnerId = new SelectList(collectService.AllMyUserList(), "Id", "Name", collect.OwnerId);
                ViewBag.RecipientId = new SelectList(collectService.AllMyUserList(), "Id", "Name", collect.RecipientId);
                return View(collect);
            }
        }

        public ActionResult Edit2(int id)
        {
            using (var collectService = new CollectsService())
                return View(collectService.GetCollect(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2([Bind(Include = "Id,OwnerId,RecipientId,Name,Description,Amount,DateOfInitiative,IsActive")] Collect collect)
        {
            if (ModelState.IsValid)
            {
                using (var collectService = new CollectsService())
                    collectService.CollectChange(collect);

                return RedirectToAction("Index");
            }
            return View(collect);
        }

        public ActionResult Delete(int id)
        {
            using (var collectService = new CollectsService())
                return View(collectService.GetCollect(id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var collectService = new CollectsService())
            {
                collectService.CollectUnActive(id);
                return RedirectToAction("Index");
            }
        }
    }
}
