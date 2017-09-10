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
using BirthdayApp.CustomFilters;

namespace BirthdayApp.Controllers
{
    [Authorize]
    [CustAuthFilter]
    public class CommonController : Controller
    {
        protected int GetUserId()
        {
            string IdentityUserId = User.Identity.GetUserId();
            using (var userService = new UsersService())
            {
                return userService.GetMyUserId(IdentityUserId);
            }
        }   
    }
}
