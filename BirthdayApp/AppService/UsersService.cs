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

        //
        //UsersController
        //

        public User GetUser(int userId)
        {
            User user = _unitOfWork.MyUserRepository.GetById(userId);
            return user;
        }

        public void UserAdd(User user)
        {
            _unitOfWork.MyUserRepository.Add(user);
            _unitOfWork.SaveChanges();
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