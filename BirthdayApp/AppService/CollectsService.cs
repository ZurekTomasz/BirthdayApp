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
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace BirthdayApp.AppService
{
    public class CollectsService : IDisposable
    {
        IUnitOfWork _unitOfWork = new UnitOfWork();

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

            foreach (var item in _unitOfWork.CollectRepository.Get().Include(c => c.Users).Where(i => i.IsActive == true))
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

            foreach (var item in _unitOfWork.CollectRepository.Get().Where(i => i.IsActive == true))
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
                foreach (var item2 in _unitOfWork.CollectUserRepository.Get().Where(i => i.CollectId == collectId && i.User.IsActive == true).ToList())
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

        public int GetCollectId(int collectUserId)
        {
            return _unitOfWork.CollectUserRepository.Get().SingleOrDefault(i => i.Id == collectUserId).CollectId.Value;
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

        //Edit
        public void CollectChange(Collect collect)
        {
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
        }

        //Create
        public List<User> AllMyUserList()
        {
            var items = _unitOfWork.MyUserRepository.Get().Where(i => i.IsActive == true).ToList();

            return items;
        }

        public void CollectAdd(Collect collect)
        {
            _unitOfWork.CollectRepository.Add(collect);
            _unitOfWork.SaveChanges();
        }

        public void CollectUnActive(int collectId)
        {
            var collect = GetCollect(collectId);

            collect.IsActive = false;
            _unitOfWork.CollectRepository.Update(collect);
            _unitOfWork.SaveChanges();
        }

        //Send Email
        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public void SendEmailsCreate(List<User> users, Collect collect)
        {
            string ownerName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(i => i.Id == collect.OwnerId).Name;
            string recipientName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(i => i.Id == collect.RecipientId).Name;
            string activeLink = GetBaseUrl() + "CollectUsers/Join/" + collect.Id.ToString() + "/";

            string subject = "Nowa zbiórka [BirthdayApp]";

            string body = "[BirthdayApp]\n" + "Została założona nowa zbiórka!" +
                "\n\nZałożyciel zbiórki: " + ownerName +
                "\n\nNazwa zbiórki: " + collect.Name +
                "\nOpis zbióki: " + collect.Description +
                "\nZbieramy dla: " + recipientName +
                "\n\nAby dołączyć do zbiórki to naciśnij na odnośnik: " + activeLink +
                "\n\n### Wiadomość została wygenerowana automatycznie, prosimy nie odpowiadać na tą wiadomość ###";

            string emailPassword = System.Configuration.ConfigurationManager.AppSettings["emailPassword"];

            foreach (var user in users)
            {
                if(!user.IgnoreEmailCreate)
                {
                    SendSingleEmail(user.Email, subject, body, "birthdayappx@gmail.com", emailPassword);
                }
                
            }
        }

        public void SendEmailsConfirm(List<User> users, Collect collect)
        {
            string ownerName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(i => i.Id == collect.OwnerId).Name;
            string recipientName = _unitOfWork.MyUserRepository.Get().SingleOrDefault(i => i.Id == collect.RecipientId).Name;
            string activeLink = GetBaseUrl() + "Collects/Details/" + collect.Id.ToString() + "/";

            string subject = "Zbiórka została zatwierdzona [BirthdayApp]";

            string body = "[BirthdayApp]\n" + "Zbiórka została zatwierdzona przez założyciela, prezent został wybrany!" +
                "\n\nZałożyciel zbiórki: " + ownerName +
                "\n\nNazwa zbiórki: " + collect.Name +
                "\nOpis zbióki: " + collect.Description +
                "\nZbieramy dla: " + recipientName +
                "\n\nAby zobaczyć szczegóły to naciśnij na odnośnik: " + activeLink +
                "\n\n### Wiadomość została wygenerowana automatycznie, prosimy nie odpowiadać na tą wiadomość ###";

            string emailPassword = System.Configuration.ConfigurationManager.AppSettings["emailPassword"];

            foreach (var user in users)
            {
                if (!user.IgnoreEmailConfirm)
                {
                    SendSingleEmail(user.Email, subject, body, "birthdayappx@gmail.com", emailPassword);
                }
            }
        }

        public void SendSingleEmail(string To, string Subject, string Body, string Email, string Password)
        {
            try
            {
                EmailViewModels email = new EmailViewModels(To, Subject, Body, Email, Password);
                using (MailMessage mm = new MailMessage(email.Email, email.To))
                {
                    mm.Subject = email.Subject;
                    mm.Body = email.Body;
                    mm.IsBodyHtml = false;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(email.Email, email.Password);
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
            catch
            {

            }
            
        }

        //
        //CollectUsersListBox
        //

        //Index
        public List<SelectListItem> AllPersonsList(int RecipientId)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (var item in _unitOfWork.MyUserRepository.Get().Where(i => i.IsActive == true).ToList())
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
            int collectUserID = _unitOfWork.CollectUserRepository.Get().SingleOrDefault(i => i.CollectId == collectId && i.UserId == userId).Id;

            _unitOfWork.CollectUserRepository.Delete(collectUserID);
            _unitOfWork.SaveChanges();
        }

        public int GetCollectUserId(int collectId, int userId)
        {
            return _unitOfWork.CollectUserRepository.Get().SingleOrDefault(i => i.CollectId == collectId && i.UserId == userId).Id;
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