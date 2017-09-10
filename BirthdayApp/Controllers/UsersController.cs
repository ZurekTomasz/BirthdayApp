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
using BirthdayApp.CustomFilters;
using PagedList;

namespace BirthdayApp.Controllers
{
    public class UsersController : CommonController
    {
        public ActionResult Index(int? page)
        {
            using (var userService = new UsersService())
            {
                ViewBag.IsAdmin = userService.IsAdmin(GetUserId());
                var users = userService.GetUserIndex();

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(users.ToPagedList(pageNumber, pageSize));
                //return View(users);
            }
        }

        public ActionResult Details(int id)
        {
            using (var userService = new UsersService())
            {
                var user = userService.GetUser(id);
                return View(user);
            }
        }

        public ActionResult Edit(int id)
        {
            using (var userService = new UsersService())
            {
                var user = userService.GetUser(id);
                return View(user);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Firstname,Surname,EntityId,Email,Role,DateOfBirth,DateOfAdd")] User user)
        {
            if (ModelState.IsValid)
            {
                using (var userService = new UsersService())
                {
                    userService.UserUpdate(user);
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult UnActive(int id)
        {
            using (var userService = new UsersService())
            {
                var user = userService.GetUser(id);
                return View(user);
            }
        }

        [HttpPost, ActionName("UnActive")]
        [ValidateAntiForgeryToken]
        public ActionResult UnActiveConfirmed(int id)
        {
            using (var userService = new UsersService())
            {
                userService.UserUnActive(id);
                return RedirectToAction("Index");
            }
        }
    }
}
