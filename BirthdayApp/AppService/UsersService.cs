using AppModels;
using BirthdayApp.Repository;
using BirthdayApp.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BirthdayApp.AppService
{
    public class UsersService : IDisposable
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
        //UsersController
        //

        public User GetUser(int userId)
        {
            User user = _unitOfWork.MyUserRepository.GetById(userId);
            return user;
        }

        public List<User> GetUserIndex()
        {
            var users = _unitOfWork.MyUserRepository.Get().Where(i => i.IsActive == true).ToList();
            return users;
        }

        public void UserUpdate(User user)
        {
            _unitOfWork.MyUserRepository.Update(user);
            _unitOfWork.SaveChanges();
        }

        public void UserUnActive(int userId)
        {
            var user = GetUser(userId);

            user.IsActive = false;
            _unitOfWork.MyUserRepository.Update(user);
            _unitOfWork.SaveChanges();
        }

        public bool IsActive(int userId)
        {
            var user = GetUser(userId);

            return user.IsActive;
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