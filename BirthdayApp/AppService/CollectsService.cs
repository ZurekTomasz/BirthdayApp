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

        public CollectViewModel UpdateCollectViewModel(int id, int userid)
        {
            var collect = GetCollect(id);

            CollectViewModel collectViewModel = new CollectViewModel();
            collectViewModel.Id = collect.Id;
            collectViewModel.UserId = userid;
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
            collectViewModel.RadioGiftItems = AllRadioGiftList(1,1).OrderByDescending(i => i.Rating).ToList();
            collectViewModel.Users = AllCollectUsersGaveMoney(collect.Id);

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


        public List<RadioGiftItem> AllRadioGiftList(int userId, int collectId)
        {
            List<RadioGiftItem> items = new List<RadioGiftItem>();

            foreach (var item in db.CollectionsGifts.Where(c=>c.CollectId == collectId))
            {
                bool val2 = false;
                if(db.CollectionsGiftRatings.Any(c => c.CollectId == collectId))
                {
                    if (db.CollectionsGiftRatings.First(c => c.CollectId == collectId).TheBestGiftId == item.Id)
                    {
                        val2 = true;
                    }
                }

                int rate = 0;

                foreach (var item3 in db.CollectionsGifts.OrderBy(i => i.Id).ToList())
                {
                    rate = 0;
                    foreach (var item2 in db.CollectionsUsers.Where(i => i.CollectId == collectId).ToList())
                    {
                        if (db.CollectionsGiftRatings.Any(i => i.CollectId == collectId && i.UserId == item2.UserId && i.TheBestGiftId == item.Id))
                        {
                            rate++;
                        }
                    }
                }


                items.Add(new RadioGiftItem
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Rating = rate,
                    Checked = val2
                });
            }
            return items;
        }

        public List<CollectUserItem> AllCollectUsersGaveMoney(int collectId)
        {
            List<CollectUserItem> items = new List<CollectUserItem>();

            foreach (var item in db.CollectionsUsers.Where(i=>i.CollectId == collectId))
            {
                items.Add(new CollectUserItem
                {
                    UserName = db.MyUsers.SingleOrDefault(i => i.Id == item.UserId).Name,
                    GaveMoney= item.GaveMoney
                });
            }
            return items;
        }

    }
}