using BirthdayApp.Models;
using System;
using System.Linq;
using AppModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using BirthdayApp.ViewModels;
using System.Collections.Generic;
using BirthdayApp.Repository;
using BirthdayApp.Repository.Contracts;

namespace BirthdayApp.AppService
{
    public class CollectsService : IDisposable
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        IUnitOfWork _unitOfWork = new UnitOfWork();

        public int GetMyUserId(string IdentityUserId)
        {
            int modelUserId = _unitOfWork.MyUserRepository.Get().Single(i => i.EntityId == IdentityUserId).Id;

            return modelUserId;
        }

        public bool IsAdmin(int userId)
        {
            if ("Admin" == _unitOfWork.MyUserRepository.Get().SingleOrDefault(i => i.Id == userId).Role)
                return true;

            return false;
        }

        public Collect GetCollect(int collectId)
        {
            Collect collect = _unitOfWork.CollectRepository.GetById(collectId);
            return collect;
        }

        public void CollectConfirmChange(int collectId, bool IsConfirm)
        {
            var collect = GetCollect(collectId);
            collect.IsConfirmed = IsConfirm;
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
            //db.Collections.Attach(collect);
            //db.Entry(collect).State = EntityState.Modified;
            //db.SaveChanges();
        }

        public void CollectAmountChange(int collectId, int Amount)
        {
            var collect = GetCollect(collectId);

            collect.Amount = Amount;
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
            //db.Collections.Attach(collect);
            //db.Entry(collect).State = EntityState.Modified;
            //db.SaveChanges();
        }

        public CollectViewModel GetCollectViewModel(int collectId, int userId)
        {
            var collect = GetCollect(collectId);

            double NumberUsersInCollect = _unitOfWork.CollectUserRepository.Get().Count(c => c.CollectId == collectId);
            double MyAmount = _unitOfWork.CollectRepository.Get().SingleOrDefault(c => c.Id == collectId).Amount;
            double AmountPerPerson = MyAmount / NumberUsersInCollect;

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
            if(_unitOfWork.CollectGiftRatingRepository.Get().Any(c => c.CollectId == collectId && c.UserId == userId))
            {
                //collectViewModel.GiftName = _unitOfWork.CollectGiftRepository.Get().Single(i => i.Id == _unitOfWork.CollectGiftRatingRepository.Get().FirstOrDefault(c => c.CollectId == collectId && c.UserId == userId).TheBestGiftId).Name;
            }
            else
            {
                collectViewModel.GiftName = "Null Gift Name";
            }
            collectViewModel.AmountPerPerson = AmountPerPerson;

            return collectViewModel;
        }


        public IEnumerable<CollectListItemViewModel> AllCollectList_v2(int userId)
        {
            HashSet<int> collectIds = new HashSet<int>();
            var query = _unitOfWork.CollectUserRepository.Get()
                           .Where(x => x.UserId == userId && x.CollectId.HasValue)
                           .Select(x => x.CollectId.Value);

            foreach (var id in query)
            {
                collectIds.Add(id);
            }

            foreach (var item in _unitOfWork.CollectRepository.Get())
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

            //foreach (var item in db.Collections.Include(c => c.Users))
            foreach(var item in _unitOfWork.CollectRepository.Get().Include(c=>c.Users))
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

            foreach (var item in _unitOfWork.CollectGiftRepository.Get().Where(i => i.CollectId == collectId))
            {
                bool isChecked = false;
                if(_unitOfWork.CollectGiftRatingRepository.Get().Any(i => i.CollectId == collectId && i.UserId == userId && i.TheBestGiftId == item.Id))
                {
                    isChecked = true;
                }

                int rating = 0;
                foreach (var item2 in _unitOfWork.CollectUserRepository.Get().Where(i => i.CollectId == collectId).ToList())
                {
                    if (_unitOfWork.CollectGiftRatingRepository.Get().Any(i => i.CollectId == collectId && i.UserId == item2.UserId && i.TheBestGiftId == item.Id))
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

            foreach (var item in _unitOfWork.CollectUserRepository.Get().Where(i => i.CollectId == collectId))
            {
                items.Add(new CollectUserItem
                {
                    UserName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(i => i.Id == item.UserId).Name,
                    GaveMoney = item.GaveMoney
                });
            }
            return items;
        }

        public bool GetPossibilityEditCollectGift(int collectId)
        {
            var CollectionsUsers = (from i in _unitOfWork.CollectUserRepository.Get()
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
            if (!_unitOfWork.CollectGiftRatingRepository.Get().Any(c => c.CollectId == collectId && c.UserId == userId))
            {
                var newCollectionGiftRatings = new CollectGiftRating();
                newCollectionGiftRatings.CollectId = collectId;
                newCollectionGiftRatings.UserId = userId;
                _unitOfWork.CollectGiftRatingRepository.Add(newCollectionGiftRatings);
                _unitOfWork.SaveChanges();
                //_context.SaveChanges();
            }

            CollectGiftRating collectionGiftRatings = _unitOfWork.CollectGiftRatingRepository.Get().SingleOrDefault(c => c.CollectId == collectId && c.UserId == userId);
            if (radioChoice != "0")
            {
                collectionGiftRatings.TheBestGiftId = Int32.Parse(radioChoice);
            }
            else
            {
                _unitOfWork.CollectGiftRatingRepository.Delete(collectionGiftRatings);
            }
            _unitOfWork.SaveChanges();
            //_context.SaveChanges();
        }

        public bool IsCollectionsUser(int collectId, int userId)
        {
            if (_unitOfWork.CollectUserRepository.Get().Any(i => i.CollectId == collectId && i.UserId == userId))
                return true;

            return false;
        }

        public int CounterCollectionsGifts(int collectId)
        {
            int result = _unitOfWork.CollectGiftRepository.Get().Count(i => i.CollectId == collectId);

            return result;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}