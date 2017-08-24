using BirthdayApp.Models;
using System;
using System.Linq;
using AppModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using BirthdayApp.ViewModels;

namespace BirthdayApp.AppService
{
    public class CollectsService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Collect GetCollect(int id)
        {
            Collect collect = db.Collections.Find(id);
            if (collect == null)
            {
                throw new Exception();
            }

            return collect;
        }

        public void CollectConfirmChange(int id, bool IsC)
        {
            var collect = GetCollect(id);

            collect.IsConfirmed = IsC;
            db.Collections.Attach(collect);
            db.Entry(collect).State = EntityState.Modified;
            db.SaveChanges();
        }

        public CollectViewModel UpdateCollectViewModel(int id)
        {
            var collect = GetCollect(id);

            CollectViewModel collectViewModel = new CollectViewModel();
            collectViewModel.Id = collect.Id;
            collectViewModel.Name = collect.Name;
            collectViewModel.OwnerId = collect.OwnerId.Value;
            collectViewModel.RecipientId = collect.RecipientId.Value;
            collectViewModel.OwnerName = collect.Owner.Name;
            collectViewModel.RecipientName = collect.Recipient.Name;
            collectViewModel.Description = collect.Description;
            collectViewModel.Amount = collect.Amount;
            collectViewModel.IsConfirmed = collect.IsConfirmed;
            collectViewModel.DateOfInitiative = collect.DateOfInitiative.Value;
            collectViewModel.DateOfAdd = collect.DateOfAdd.Value;

            return collectViewModel;
        }
    }
}