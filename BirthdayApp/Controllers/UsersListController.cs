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
                
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                    ViewBag.Message += "\\n" + selectedItem.Text;
                }

                foreach (var item in person.UsersList)
                {
                    if (!item.Selected)
                    {
                        db.CollectionsUsers.Remove(db.CollectionsUsers.Find(db.CollectionsUsers.SingleOrDefault(c => c.UserId.ToString() == item.Value).Id));
                    }
                    else
                    {
                        var newCollectionUsers = new CollectUser();
                        newCollectionUsers.UserId = Int32.Parse(item.Value);
                        newCollectionUsers.CollectId = 1;
                        db.CollectionsUsers.Add(newCollectionUsers);
                    }
                    db.SaveChanges();
                }

                //foreach (var nw in person.UsersList)
                //{
                //    if (!nw.Selected)
                //        ViewBag.a1 = "1";

                //    //if (nw.pol.Selected)
                //    //    ViewBag.a2 = "2";
                //}

                //foreach (var item in person.UsersList)
                //{
                //    if (item.Selected == false && !selectedItems.Any(c => c.Selected == item.Selected))
                //    {
                //        //var newCollectionUsers = new CollectUser();
                //        //newCollectionUsers.UserId = Int32.Parse(item.Value);
                //        //newCollectionUsers.CollectId = 1;
                //        //db.CollectionsUsers.Add(newCollectionUsers);
                //    }
                //    else
                //    {
                //        //ViewBag.rmv = db.CollectionsUsers.SingleOrDefault(c => c.CollectId == 1 && c.UserId.ToString() == item.Value).Id;
                //        //db.CollectionsUsers.Remove(db.CollectionsUsers.Find(db.CollectionsUsers.SingleOrDefault(c => c.CollectId == 1 && c.UserId.ToString() == item.Value && item.Selected == true).Id));


                //    }

                //    db.SaveChanges();
                //}


                //ViewBag.Message = "Wybrani użytkownicy:";





                //var newCollectionUsers = new CollectUser();
                //newCollectionUsers.UserId = 1;
                //newCollectionUsers.CollectId = 1;
                //context.CollectionsUsers.Add(newCollectionUsers);
                //context.SaveChanges();


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