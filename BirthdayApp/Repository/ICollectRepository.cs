using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppModels;

namespace BirthdayApp.Repository
{
    public interface ICollectRepository : IDisposable
    {
        IEnumerable<Collect> GetAllCollect();
        IEnumerable<Collect> GetAllCollectIncludeUsers();
        Collect GetCollectById(int studentId);
        int AddCollect(Collect employeeEntity);
        int UpdateCollect(Collect employeeEntity);
        void DeleteCollect(int CollectId);
    }
}