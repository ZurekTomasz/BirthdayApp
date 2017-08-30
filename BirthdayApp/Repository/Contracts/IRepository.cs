using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayApp.Repository.Contracts
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> Get();
        TEntity GetById(object entityId);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(object entityId);
        void DeleteRange(IQueryable<TEntity> entity);
    }
}
