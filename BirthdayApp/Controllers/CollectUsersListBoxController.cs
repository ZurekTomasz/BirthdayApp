using AppModels;
using BirthdayApp.AppService;
using BirthdayApp.Models;
using BirthdayApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BirthdayApp.Controllers
{
    public class CollectUsersListBoxController : CommonController
    {
        public ActionResult Index(int id)
        {
            using (var collectService = new CollectsService())
            {
                var person = collectService.GetCollectUsersListBoxViewModel(id);
                return View(person);
            }
        }

        [HttpPost]
        public ActionResult Index(int id, CollectUsersListBoxViewModel person)
        {
            using (var collectService = new CollectsService())
            {
                var personPost = collectService.GetCollectUsersListBoxViewModelPost(id,person);
                return View(personPost);
            }
        }

    }
}