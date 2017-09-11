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
    public class CollectUsersController : CommonController
    {
        public ActionResult Index(int id)
        {
            using (var collectService = new CollectsService())
            {
                using (var userService = new UsersService())
                {
                    if (!userService.IsOwner(GetUserId()))
                        return HttpNotFound();
                }

                ViewBag.ThisId = id;
                var collectUsers = collectService.GetCollectUserIndex(id);
                
                return View(collectUsers);
            }
        }

        public ActionResult GaveMoney(int id)
        {
            using (var collectService = new CollectsService())
            {
                collectService.GaveMoneyChange(id);
                return RedirectToAction("Index", "CollectUsers", new { id = id });
            }
        }

        public ActionResult Join(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectViewModel = collectService.GetCollectViewModel(id, GetUserId());
                return View(collectViewModel);
            }
        }

        [HttpPost, ActionName("Join")]
        [ValidateAntiForgeryToken]
        public ActionResult JoinConfirmed(int id)
        {
            using (var collectService = new CollectsService())
            {
                int userId = GetUserId();
                var collectUser = collectService.GetCollectUser(id);

                if (ModelState.IsValid)
                {
                    collectService.JoinConfirmed(id, GetUserId());

                    return RedirectToAction("Details", "Collects" , new { id = collectUser.CollectId });
                }
            }

            return HttpNotFound();
        }
        
        public ActionResult Leave(int id)
        {
            using (var collectService = new CollectsService())
            {
                var collectViewModel = collectService.GetCollectViewModel(id,GetUserId());
                return View(collectViewModel);
            }
        }

        [HttpPost, ActionName("Leave")]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveConfirmed(int id)
        {
            using (var collectService = new CollectsService())
            {
                collectService.LeaveConfirmed(id, GetUserId());
                return RedirectToAction("Index", "Collects");
            }
        }
    }
}
