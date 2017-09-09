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
using System.Web.Mvc;

namespace BirthdayApp.AppService
{
    public class CollectsService : IDisposable
    {
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

        public bool IsOwner(int userId)
        {
            if (_unitOfWork.CollectUserRepository.Get().Any(c => c.Collect.OwnerId == userId))
                return true;

            return false;
        }

        //
        //CollectsController
        //

        public Collect GetCollect(int collectId)
        {
            Collect collect = _unitOfWork.CollectRepository.GetById(collectId);
            return collect;
        }

        //Index
        public List<CollectListItemViewModel> AllCollectList(int userId)
        {
            List<CollectListItemViewModel> items = new List<CollectListItemViewModel>();

            foreach (var item in _unitOfWork.CollectRepository.Get().Include(c => c.Users))
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

        //Details
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
            if (_unitOfWork.CollectGiftRatingRepository.Get().Any(c => c.CollectId == collectId && c.UserId == userId && c.TheBestGiftId != null))
            {
                int GiftId = _unitOfWork.CollectGiftRatingRepository.Get().FirstOrDefault(c => c.CollectId == collectId && c.UserId == userId).TheBestGiftId.Value;
                collectViewModel.GiftName = _unitOfWork.CollectGiftRepository.Get().Single(i => i.Id == GiftId).Name;
            }
            else
            {
                collectViewModel.GiftName = "Null Gift Name";
            }
            collectViewModel.AmountPerPerson = AmountPerPerson;

            return collectViewModel;
        }

        public List<RadioGiftItem> AllRadioGiftList(int collectId, int userId)
        {
            List<RadioGiftItem> items = new List<RadioGiftItem>();

            foreach (var item in _unitOfWork.CollectGiftRepository.Get().Where(i => i.CollectId == collectId))
            {
                bool isChecked = false;
                if (_unitOfWork.CollectGiftRatingRepository.Get().Any(i => i.CollectId == collectId && i.UserId == userId && i.TheBestGiftId == item.Id))
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
            if (GaveMoneyCounter > 0)
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
            }

            CollectGiftRating collectionGiftRatings = _unitOfWork.CollectGiftRatingRepository.Get().SingleOrDefault(c => c.CollectId == collectId && c.UserId == userId);
            if (radioChoice != "0")
            {
                collectionGiftRatings.TheBestGiftId = Int32.Parse(radioChoice);
            }
            else
            {
                _unitOfWork.CollectGiftRatingRepository.Delete(collectionGiftRatings.Id);
            }
            _unitOfWork.SaveChanges();
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

        //Confirm
        public void CollectConfirmChange(int collectId, bool IsConfirm)
        {
            var collect = GetCollect(collectId);
            collect.IsConfirmed = IsConfirm;
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
        }

        public void CollectAmountChange(int collectId, int Amount)
        {
            var collect = GetCollect(collectId);

            collect.Amount = Amount;
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
        }


        //Edit2
        public void CollectChange(Collect collect)
        {
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
        }

        //Create
        public List<User> AllMyUserList()
        {
            var items = _unitOfWork.MyUserRepository.Get().ToList();

            return items;
        }

        public void CollectAdd(Collect collect)
        {
            _unitOfWork.CollectRepository.Add(collect);
            _unitOfWork.SaveChanges();
        }

        //Delete
        public void CollectDelete(int id)
        {
            var collect = GetCollect(id);
            var collectGift = _unitOfWork.CollectGiftRepository.Get().Where(x => x.CollectId == collect.Id);
            foreach (var item in collectGift.ToList())
            {
                var collectGiftRating = _unitOfWork.CollectGiftRatingRepository.Get().Where(x => x.TheBestGiftId == item.Id);
                _unitOfWork.CollectGiftRatingRepository.DeleteRange(collectGiftRating);
                _unitOfWork.SaveChanges();
            }
            var collectUser = _unitOfWork.CollectUserRepository.Get().Where(x => x.CollectId == collect.Id);
            _unitOfWork.CollectGiftRepository.DeleteRange(collectGift);
            _unitOfWork.CollectUserRepository.DeleteRange(collectUser);
            _unitOfWork.CollectRepository.Delete(collect.Id);

            _unitOfWork.SaveChanges();
        }

        //
        //CollectUsersListBox
        //

        //Index
        public List<SelectListItem> AllPersonsList(int RecipientId)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in _unitOfWork.MyUserRepository.Get().ToList())
            {
                if (item.Id != RecipientId)
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

        public CollectUsersListBoxViewModel GetCollectUsersListBoxViewModel(int collectId)
        {
            var collect = GetCollect(collectId);
            int RecipientId = _unitOfWork.MyUserRepository.Get().SingleOrDefault(c => c.Id == collect.RecipientId).Id;

            CollectUsersListBoxViewModel person = new CollectUsersListBoxViewModel();
            using (var collectService = new CollectsService())
            {
                person.UsersList = collectService.AllPersonsList(RecipientId);
                person.CollectId = collect.Id;
                person.CollectName = collect.Name;
                person.RecipientName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(c => c.Id == collect.RecipientId).Name;
            }

            foreach (var item in person.UsersList)
            {
                item.Selected = _unitOfWork.CollectUserRepository.Get().Any(c => c.CollectId == collect.Id && c.UserId.ToString() == item.Value);
            }

            return person;
        }

        public CollectUsersListBoxViewModel GetCollectUsersListBoxViewModelPost(int collectId, CollectUsersListBoxViewModel person)
        {
            Collect collect = GetCollect(collectId);

            int RecipientId = _unitOfWork.MyUserRepository.Get().SingleOrDefault(c => c.Id == collect.RecipientId).Id;
            using (var collectService = new CollectsService())
            {
                person.UsersList = collectService.AllPersonsList(RecipientId);
                person.CollectId = collect.Id;
                person.CollectName = collect.Name;
                person.RecipientName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(c => c.Id == collect.RecipientId).Name;
            }
            if (person.UsersListIds != null)
            {
                List<SelectListItem> selectedItems = person.UsersList.Where(p => person.UsersListIds.Contains(int.Parse(p.Value))).ToList();

                //ViewBag.Message = "Wybrani użytkownicy:";
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                    //ViewBag.Message += "\\n" + selectedItem.Text;

                    if (!_unitOfWork.CollectUserRepository.Get().Any(c => c.UserId.ToString() == selectedItem.Value && c.CollectId == collect.Id))
                    {
                        var newCollectionUsers = new CollectUser();
                        newCollectionUsers.UserId = Int32.Parse(selectedItem.Value);
                        newCollectionUsers.CollectId = collect.Id;
                        _unitOfWork.CollectUserRepository.Add(newCollectionUsers);
                        _unitOfWork.SaveChanges();
                    }
                }

                foreach (var item in person.UsersList)
                {
                    if (!item.Selected)
                    {
                        if (_unitOfWork.CollectUserRepository.Get().Any(c => c.UserId.ToString() == item.Value && c.CollectId == collect.Id))
                        {
                            _unitOfWork.CollectUserRepository.Delete(_unitOfWork.CollectUserRepository.Get().SingleOrDefault(c => c.UserId.ToString() == item.Value && c.CollectId == collect.Id).Id);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                foreach (var item in person.UsersList)
                {
                    if (!item.Selected)
                    {
                        if (_unitOfWork.CollectUserRepository.Get().Any(c => c.UserId.ToString() == item.Value && c.CollectId == collect.Id))
                        {
                            _unitOfWork.CollectUserRepository.Delete(_unitOfWork.CollectUserRepository.Get().SingleOrDefault(c => c.UserId.ToString() == item.Value && c.CollectId == collect.Id).Id);
                            _unitOfWork.SaveChanges();
                        }
                    }
                }
            }

            return person;
        }

        //
        //CollectsGiftsController
        //

        public CollectGift GetCollectGift(int collectGiftId)
        {
            CollectGift collectGift = _unitOfWork.CollectGiftRepository.GetById(collectGiftId);
            return collectGift;
        }

        //Index
        public List<CollectGift> GetCollectsGiftsIndex(int collectId)
        {
            var collectionsGifts = _unitOfWork.CollectGiftRepository.Get().Include(c => c.Collect).Include(c => c.User).Where(c => c.CollectId == collectId).ToList();

            return collectionsGifts;
        }

        //Create
        public void CollectGiftAdd(CollectGift collectGift)
        {
            _unitOfWork.CollectGiftRepository.Add(collectGift);
            _unitOfWork.SaveChanges();
        }

        //Delete
        public void CollectGiftDelete(int collectGiftId)
        {
            _unitOfWork.CollectGiftRepository.Delete(collectGiftId);
            _unitOfWork.SaveChanges();
        }

        //
        //CollectsUsersController
        //

        public CollectUser GetCollectUser(int collectuserId)
        {
            CollectUser collectuser = _unitOfWork.CollectUserRepository.GetById(collectuserId);
            return collectuser;
        }

        public void CollectUserAdd(CollectUser collectuser)
        {
            _unitOfWork.CollectUserRepository.Add(collectuser);
            _unitOfWork.SaveChanges();
        }

        public List<CollectUser> GetCollectUserIndex(int collectuserId)
        {
            var collectionsUsers = _unitOfWork.CollectUserRepository.Get().Include(c => c.Collect).Include(c => c.User).Where(c => c.CollectId == collectuserId).ToList();
            return collectionsUsers;
        }

        public void GaveMoneyChange(int collectuserId)
        {
            var collectUser = GetCollectUser(collectuserId);

            if (collectUser.GaveMoney)
            {
                collectUser.GaveMoney = false;
            }
            else
            {
                collectUser.GaveMoney = true;
            }

            _unitOfWork.CollectUserRepository.Update(collectUser);
            _unitOfWork.SaveChanges();
        }

        public void JoinConfirmed(int collectId, int userId)
        {
            if (!_unitOfWork.CollectUserRepository.Get().Any(i => i.CollectId == collectId && i.UserId == userId))
            {
                CollectUser collectUser = new CollectUser();
                collectUser.UserId = userId;
                collectUser.CollectId = collectId;
                collectUser.GaveMoney = false;
                _unitOfWork.CollectUserRepository.Add(collectUser);
                _unitOfWork.SaveChanges();
            }
        }

        public void LeaveConfirmed(int collectId, int userId)
        {
            int collectUsersID = _unitOfWork.CollectUserRepository.Get().SingleOrDefault(i => i.CollectId == collectId && i.UserId == userId).Id;

            CollectUser collectUser = _unitOfWork.CollectUserRepository.GetById(collectUsersID);
            _unitOfWork.CollectUserRepository.Delete(collectUser);
            _unitOfWork.SaveChanges();
        }

        //
        //Disposed
        //
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