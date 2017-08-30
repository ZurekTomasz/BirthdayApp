using AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayApp.Repository.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Collect> CollectRepository { get; }
        IRepository<CollectUser> CollectUserRepository { get; }

        // etc wszystkie repo

        int SaveChanges();
    }
}
