using BirthdayApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BirthdayApp.Controllers
{
    public class UsersListController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            UsersListBox fruit = new UsersListBox();
            fruit.UsersList = PopulateFruits();
            return View(fruit);
        }

        [HttpPost]
        public ActionResult Index(UsersListBox fruit)
        {
            fruit.UsersList = PopulateFruits();
            if (fruit.UsersListIds != null)
            {
                List<SelectListItem> selectedItems = fruit.UsersList.Where(p => fruit.UsersListIds.Contains(int.Parse(p.Value))).ToList();

                ViewBag.Message = "Selected Fruits:";
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                    ViewBag.Message += "\\n" + selectedItem.Text;
                }
            }

            return View(fruit);
        }

        private static List<SelectListItem> PopulateFruits()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Pies",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Kot",
                Value = "2"
            });

            items.Add(new SelectListItem
            {
                Text = "Małpa",
                Value = "3"
            });

            return items;
        }
    }
}