using BirthdayApp.Models;
using System;
using System.Linq;
using AppModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using BirthdayApp.ViewModels;
using System.Collections.Generic;

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


        public IEnumerable<CollectListItemViewModel> AllCollectList_v2(int userId)
        {
            HashSet<int> collectIds = new HashSet<int>();
            var query = db.CollectionsUsers
                           .Where(x => x.UserId == userId && x.CollectId.HasValue)
                           .Select(x => x.CollectId.Value);

            foreach (var id in query)
            {
                collectIds.Add(id);
            }

            foreach (var item in db.Collections)
            {
                CollectListItemViewModel collectItem = new CollectListItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    UserId = userId,
                    OwnerId = item.OwnerId.Value,
                    RecipientId = item.RecipientId.Value,
                    OwnerName = item.Owner.Name,
                    RecipientName = item.Recipient.Name,
                    Description = item.Description,
                    Amount = item.Amount,
                    IsConfirmed = item.IsConfirmed,
                    DateOfInitiative = item.DateOfInitiative.Value,
                    DateOfAdd = item.DateOfAdd.Value,
                    YoureInCollection = collectIds.Contains(item.Id)
                };
                yield return collectItem;
            }
        }

        public List<CollectListItemViewModel> AllCollectList(int userId)
        {
            List<CollectListItemViewModel> items = new List<CollectListItemViewModel>();

            foreach (var item in db.Collections.Include(c => c.Users))
            {
                items.Add(new CollectListItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    UserId = userId,
                    OwnerId = item.OwnerId.Value,
                    RecipientId = item.RecipientId.Value,
                    OwnerName = item.Owner.Name,
                    RecipientName = item.Recipient.Name,
                    Description = item.Description,
                    Amount = item.Amount,
                    IsConfirmed = item.IsConfirmed,
                    DateOfInitiative = item.DateOfInitiative.Value,
                    DateOfAdd = item.DateOfAdd.Value,
                    YoureInCollection = item.Users.Any(cu => cu.UserId == userId)
                });
            }
            return items;
        }
    }
}