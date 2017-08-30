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
    public class CollectsRepository : ICollectsRepository
    {
        private readonly ApplicationDbContext _context;

        public CollectsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Collect> GetAllCollect()
        {
            return _context.Collections.ToList();
        }

        public IEnumerable<Collect> GetAllCollectIncludeUsers()
        {
            return _context.Collections.Include(i => i.Users).ToList();
        }

        public Collect GetCollectById(int collectId)
        {
            return _context.Collections.Find(collectId);
        }

        public int AddCollect(Collect collect)

        {
            int result = -1;

            if (collect != null)
            {
                _context.Collections.Add(collect);
                _context.SaveChanges();
                result = collect.Id;
            }
            return result;

        }
        public int UpdateCollect(Collect collect)
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
        public void DeleteCollect(int collectId)
        {
            Collect collect = _context.Collections.Find(collectId);
            _context.Collections.Remove(collect);
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