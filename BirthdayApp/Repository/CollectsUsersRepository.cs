using AppModels;
using BirthdayApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BirthdayApp.Repository
{
    public class CollectsUsersRepository : ICollectsUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public CollectsUsersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<CollectUser> GetAllCollectUser()
        {
            return _context.CollectionsUsers.ToList();
        }

        public CollectUser GetCollectUserById(int collectId)
        {
            return _context.CollectionsUsers.Find(collectId);
        }

        public int AddCollectUser(CollectUser collect)

        {
            int result = -1;

            if (collect != null)
            {
                _context.CollectionsUsers.Add(collect);
                _context.SaveChanges();
                result = collect.Id;
            }
            return result;

        }
        public int UpdateCollectUser(CollectUser collect)
        {
            int result = -1;

            if (collect != null)
            {
                _context.Entry(collect).State = EntityState.Modified;
                _context.SaveChanges();
                result = collect.Id;
            }
            return result;
        }
        public void DeleteCollectUser(int collectId)
        {
            CollectUser collect = _context.CollectionsUsers.Find(collectId);
            _context.CollectionsUsers.Remove(collect);
            _context.SaveChanges();

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