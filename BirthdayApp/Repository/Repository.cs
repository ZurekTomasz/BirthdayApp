using BirthdayApp.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using BirthdayApp.Models;
using System.Data.Entity;

namespace BirthdayApp.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Get()
        {
            return _context.Set<TEntity>();
        }

        public TEntity GetById(object entityId)
        {
            return _context.Set<TEntity>().Find(entityId);
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Entry<TEntity>(entity).State = EntityState.Modified;
        }

        public void Delete(object entityId)
        {
            var entity = _context.Set<TEntity>().Find(entityId);
            _context.Set<TEntity>().Remove(entity);
        }
    }
}