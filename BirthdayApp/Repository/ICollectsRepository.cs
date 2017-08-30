using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppModels;

namespace BirthdayApp.Repository
{
    public interface ICollectsRepository : IDisposable
    {
        IEnumerable<Collect> GetAllCollect();
        IEnumerable<Collect> GetAllCollectIncludeUsers();
        Collect GetCollectById(int collectId);
        int AddCollect(Collect collect);
        int UpdateCollect(Collect collect);
        void DeleteCollect(int collectId);
    }
}