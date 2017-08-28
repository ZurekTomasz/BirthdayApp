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

        public Collect GetCollect(int collectId)
        {
            Collect collect = db.Collections.Find(collectId);
            if (collect == null)
            {
                throw new Exception();
            }

            return collect;
        }

        public void CollectConfirmChange(int collectId, bool IsConfirm)
        {
            var collect = GetCollect(collectId);

            collect.IsConfirmed = IsConfirm;
            db.Collections.Attach(collect);
            db.Entry(collect).State = EntityState.Modified;
            db.SaveChanges();
        }

        public CollectViewModel UpdateCollectViewModel(int collectId, int userId)
        {
            var collect = GetCollect(collectId);

            decimal NumberUsersInCollect = db.CollectionsUsers.Count(c => c.CollectId == collectId);
            decimal MyAmount = db.Collections.SingleOrDefault(c => c.Id == collectId).Amount;
            decimal AmountPerPerson = MyAmount / NumberUsersInCollect;

            CollectViewModel collectViewModel = new CollectViewModel();
            collectViewModel.Id = collect.Id;
            collectViewModel.UserId = userId;
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
            collectViewModel.RadioGiftItems = AllRadioGiftList(collectId, userId).OrderByDescending(i => i.Rating).ToList();
            collectViewModel.Users = AllCollectUsersGaveMoney(collect.Id);
            collectViewModel.PossibilityEditCollectGift = GetPossibilityEditCollectGift(collectId);
            //collectViewModel.GiftName = db.CollectionsGifts.SingleOrDefault(i => i.Id == db.CollectionsGiftRatings.FirstOrDefault(c => c.CollectId == collectId && c.UserId == userId).Id).Name;
            collectViewModel.GiftName = "Example GiftName";
            collectViewModel.AmountPerPerson = AmountPerPerson;

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
                CollectListItemViewModel items = new CollectListItemViewModel
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
                yield return items;
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


        public List<RadioGiftItem> AllRadioGiftList(int collectId, int userId)
        {
            List<RadioGiftItem> items = new List<RadioGiftItem>();

            foreach (var item in db.CollectionsGifts.Where(i => i.CollectId == collectId))
            {
                bool isChecked = false;
                if(db.CollectionsGiftRatings.Any(i => i.CollectId == collectId && i.UserId == userId && i.TheBestGiftId == item.Id))
                {
                    isChecked = true;
                }

                int rating = 0;
                foreach (var item2 in db.CollectionsUsers.Where(i => i.CollectId == collectId).ToList())
                {
                    if (db.CollectionsGiftRatings.Any(i => i.CollectId == collectId && i.UserId == item2.UserId && i.TheBestGiftId == item.Id))
                    {
                        rating++;
                    }
                }

                items.Add(new RadioGiftItem
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Rating = rating,
                    Checked = isChecked
                });
            }
            return items;
        }

        public List<CollectUserItem> AllCollectUsersGaveMoney(int collectId)
        {
            List<CollectUserItem> items = new List<CollectUserItem>();

            foreach (var item in db.CollectionsUsers.Where(i => i.CollectId == collectId))
            {
                items.Add(new CollectUserItem
                {
                    UserName = db.MyUsers.SingleOrDefault(i => i.Id == item.UserId).Name,
                    GaveMoney = item.GaveMoney
                });
            }
            return items;
        }

        public bool GetPossibilityEditCollectGift(int collectId)
        {
            var CollectionsUsers = (from i in db.CollectionsUsers
                                    where i.CollectId == collectId
                                    select i).ToList();

            int GaveMoneyCounter = 0;
            foreach (var item in CollectionsUsers)
            {
                if (item.GaveMoney)
                {
                    GaveMoneyCounter++;
                }
            }

            bool result = true;
            if(GaveMoneyCounter > 0)
            {
                result = false;
            }

            return result;
        }

        public void ChangeRadioButtonChoose(int collectId, int userId, string radioChoice)
        {
            if (!db.CollectionsGiftRatings.Any(c => c.CollectId == collectId && c.UserId == userId))
            {
                var newCollectionGiftRatings = new CollectGiftRating();
                newCollectionGiftRatings.CollectId = collectId;
                newCollectionGiftRatings.UserId = userId;
                db.CollectionsGiftRatings.Add(newCollectionGiftRatings);
                db.SaveChanges();
            }

            CollectGiftRating collectionGiftRatings = db.CollectionsGiftRatings.SingleOrDefault(c => c.CollectId == collectId && c.UserId == userId);
            if (radioChoice != "0")
            {
                collectionGiftRatings.TheBestGiftId = Int32.Parse(radioChoice);
            }
            else
            {
                db.CollectionsGiftRatings.Remove(collectionGiftRatings);
            }
            db.SaveChanges();
        }

    }
}