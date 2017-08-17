using AppModels;
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
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home
        public ActionResult Index()
        {
            UsersListBox person = new UsersListBox();
            person.UsersList = AllPersonsList();
            
            foreach(var item in person.UsersList)
            {
                item.Selected = db.CollectionsUsers.Any(collect => collect.CollectId == 1 && collect.UserId.ToString() == item.Value);
            }

            return View(person);
        }

        [HttpPost]
        public ActionResult Index(UsersListBox person)
        {
            person.UsersList = AllPersonsList();
            if (person.UsersListIds != null)
            {
                List<SelectListItem> selectedItems = person.UsersList.Where(p => person.UsersListIds.Contains(int.Parse(p.Value))).ToList();

                

                ViewBag.Message = "Wybrani użytkownicy:";
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                    ViewBag.Message += "\\n" + selectedItem.Text;

                    if(!db.CollectionsUsers.Any(c => c.UserId.ToString() == selectedItem.Value))
                    {
                        var newCollectionUsers = new CollectUser();
                        newCollectionUsers.UserId = Int32.Parse(selectedItem.Value);
                        newCollectionUsers.CollectId = 1;
                        db.CollectionsUsers.Add(newCollectionUsers);
                        db.SaveChanges();
                    }
                }

                foreach (var item in person.UsersList)
                {
                    if(!item.Selected)
                    {
                        if(db.CollectionsUsers.Any(c => c.UserId.ToString() == item.Value))
                        {
                            db.CollectionsUsers.Remove(db.CollectionsUsers.Find(db.CollectionsUsers.SingleOrDefault(c => c.UserId.ToString() == item.Value).Id));
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                foreach (var item in person.UsersList)
                {
                    if (!item.Selected )
                    {
                        if (db.CollectionsUsers.Any(c => c.UserId.ToString() == item.Value))
                        {
                            db.CollectionsUsers.Remove(db.CollectionsUsers.Find(db.CollectionsUsers.SingleOrDefault(c => c.UserId.ToString() == item.Value).Id));
                            db.SaveChanges();
                        }
                    }
                }
            }

            return View(person);
        }

        private List<SelectListItem> AllPersonsList()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            
            foreach (var item in db.ModelUsers.ToList())
            {
                items.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                    //Selected = item.CollectUsers.Any(collect => collect.CollectId == 1)
                });
            }
            return items;
        }
    }
}