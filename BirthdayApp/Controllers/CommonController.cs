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
    [Authorize]
    public class CommonController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        protected string GetUserId()
        {
            string userId = User.Identity.GetUserId();
            return userId;
        }

        protected int GetModelUserId()
        {
            string userId = User.Identity.GetUserId();
            int modelUserId = db.MyUsers.Single(i => i.EntityId == userId).Id;

            return modelUserId;
        }

        protected bool IsAdmin()
        {
            int UserId = GetModelUserId();
            if ("Admin" == db.MyUsers.SingleOrDefault(i => i.Id == UserId).Role)
            {
                return true;
            }

            return false;
        }
        
    }
}
