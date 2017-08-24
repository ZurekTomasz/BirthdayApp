using AppModels;
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
    public class UsersListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Collect collect = db.Collections.Find(id);
            if (collect == null)
            {
                return HttpNotFound();
            }

            ViewBag.id = collect.Id;
            ViewBag.collectName = collect.Name;
            ViewBag.recipientName = db.ModelUsers.SingleOrDefault(c => c.Id == collect.RecipientId).Name;
            int RecipientId = db.ModelUsers.SingleOrDefault(c => c.Id == collect.RecipientId).Id;

            UsersListBox person = new UsersListBox();
            person.UsersList = AllPersonsList(RecipientId);

            foreach(var item in person.UsersList)
            {
                    item.Selected = db.CollectionsUsers.Any(c => c.CollectId == id && c.UserId.ToString() == item.Value);
            }

            return View(person);
        }

        [HttpPost]
        public ActionResult Index(int? id, UsersListBox person)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Collect collect = db.Collections.Find(id);
            if (collect == null)
            {
                return HttpNotFound();
            }

            ViewBag.id = collect.Id;
            ViewBag.collectName = collect.Name;
            ViewBag.recipientName = db.ModelUsers.SingleOrDefault(c => c.Id == collect.RecipientId).Name;

            int RecipientId = db.ModelUsers.SingleOrDefault(c => c.Id == collect.RecipientId).Id;
            person.UsersList = AllPersonsList(RecipientId);
            if (person.UsersListIds != null)
            {
                List<SelectListItem> selectedItems = person.UsersList.Where(p => person.UsersListIds.Contains(int.Parse(p.Value))).ToList();

                ViewBag.Message = "Wybrani użytkownicy:";
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                    ViewBag.Message += "\\n" + selectedItem.Text;

                    if (!db.CollectionsUsers.Any(c => c.UserId.ToString() == selectedItem.Value && c.CollectId == id))
                    {
                        var newCollectionUsers = new CollectUser();
                        newCollectionUsers.UserId = Int32.Parse(selectedItem.Value);
                        newCollectionUsers.CollectId = id;
                        db.CollectionsUsers.Add(newCollectionUsers);
                        db.SaveChanges();
                    }
                }

                foreach (var item in person.UsersList)
                {
                    if(!item.Selected)
                    {
                        if(db.CollectionsUsers.Any(c => c.UserId.ToString() == item.Value && c.CollectId == id))
                        {
                            db.CollectionsUsers.Remove(db.CollectionsUsers.Find(db.CollectionsUsers.SingleOrDefault(c => c.UserId.ToString() == item.Value && c.CollectId == id).Id));
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
                        if (db.CollectionsUsers.Any(c => c.UserId.ToString() == item.Value && c.CollectId == id))
                        {
                            db.CollectionsUsers.Remove(db.CollectionsUsers.Find(db.CollectionsUsers.SingleOrDefault(c => c.UserId.ToString() == item.Value && c.CollectId == id).Id));
                            db.SaveChanges();
                        }
                    }
                }
            }

            return View(person);
        }

        public ActionResult List()
        {

            return View();
        }

        private List<SelectListItem> AllPersonsList(int RecipientId)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in db.ModelUsers.ToList())
            {
                if(item.Id != RecipientId)
                {
                    items.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                        //Selected = item.CollectUsers.Any(collect => collect.CollectId == 1)
                    });
                }
            }
            return items;
        }
    }
}