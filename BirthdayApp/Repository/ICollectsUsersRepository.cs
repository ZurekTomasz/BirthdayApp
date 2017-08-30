using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppModels;

namespace BirthdayApp.Repository
{
    public interface ICollectsUsersRepository : IDisposable
    {
        IEnumerable<CollectUser> GetAllCollectUser();
        CollectUser GetCollectUserById(int collectuserId);
        int AddCollectUser(CollectUser collectuser);
        int UpdateCollectUser(CollectUser collectuser);
        void DeleteCollectUser(int collectuserId);
    }
}