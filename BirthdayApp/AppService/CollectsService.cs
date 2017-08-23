using BirthdayApp.Models;
using System;
using System.Linq;
using AppModels;
using System.Data.Entity;
using Microsoft.AspNet.Identity;


namespace BirthdayApp.AppService
{
    public class CollectsService
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Collect GetCollect(int id)
        {
            Collect collect = db.Collections.Find(id);
            if (collect == null)
            {
                throw new Exception();
            }

            return collect;
        }

        public void CollectConfirmChange(int id, bool IsC)
        {
            var collect = GetCollect(id);

            collect.IsConfirmed = IsC;
            db.Collections.Attach(collect);
            db.Entry(collect).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}