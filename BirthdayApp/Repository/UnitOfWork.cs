using BirthdayApp.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppModels;
using BirthdayApp.Models;

namespace BirthdayApp.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<Collect> _collectRepository;
        public IRepository<Collect> CollectRepository
        {
            get
            {
                if (_collectRepository == null)
                {
                    _collectRepository = new Repository<Collect>(_context);
                }

                return _collectRepository;
            }
        }

        IRepository<CollectUser> _collectUserRepository;
        public IRepository<CollectUser> CollectUserRepository
        {
            get
            {
                if (_collectUserRepository == null)
                {
                    _collectUserRepository = new Repository<CollectUser>(_context);
                }

                return _collectUserRepository;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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