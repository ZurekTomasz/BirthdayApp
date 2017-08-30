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
    [Authorize]
    public class CommonController : Controller
    {
        protected int GetUserId()
        {
            string IdentityUserId = User.Identity.GetUserId();
            using (var collectService = new CollectsService())
            {
                return collectService.GetMyUserId(IdentityUserId);
            }
        }   
    }
}
