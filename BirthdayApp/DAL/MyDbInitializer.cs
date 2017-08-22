using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BirthdayApp.Models;
using AppModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace BirthdayApp.DAL
{
    public class MyDbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            string fName, sName, fEmail, Password, Role;
            DateTime bDate;

            //Tomasz Żurek
            fName = "Tomasz";
            sName = "Żurek";
            fEmail = "mail@tomass.net";
            Password = "123456";
            Role = "Admin";
            bDate = DateTime.ParseExact("1997-05-20", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var store = new UserStore<ApplicationUser>(context);
            var manager = new UserManager<ApplicationUser>(store);
            var user = new ApplicationUser { UserName = fEmail, Email = fEmail };
            var result = manager.Create(user, Password);

            if (result.Succeeded)
            {
                var mUser = new ModelUser(fName, sName, fEmail, Role, bDate, user.Id);
                context.ModelUsers.Add(mUser);
                context.SaveChanges();
          
            }
            //**//

            //Aleksander Tabor
            fName = "Aleksander";
            sName = "Tabor";
            fEmail = "aleksander@gmail.com";
            Password = "123456";
            Role = "User";
            bDate = DateTime.ParseExact("1996-01-26", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var store2 = new UserStore<ApplicationUser>(context);
            var manager2 = new UserManager<ApplicationUser>(store);
            var user2 = new ApplicationUser { UserName = fEmail, Email = fEmail };
            var result2 = manager.Create(user2, Password);

            if (result2.Succeeded)
            {
                var mUser2 = new ModelUser(fName, sName, fEmail, Role, bDate, user2.Id);
                context.ModelUsers.Add(mUser2);
                context.SaveChanges();
            }
            //**//

            //**//

            //Michał Paszkowiak
            fName = "Michał";
            sName = "Paszkowiak";
            fEmail = "michal@gmail.com";
            Password = "123456";
            Role = "User";
            bDate = DateTime.ParseExact("1997-12-28", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var store3 = new UserStore<ApplicationUser>(context);
            var manager3 = new UserManager<ApplicationUser>(store);
            var user3 = new ApplicationUser { UserName = fEmail, Email = fEmail };
            var result3 = manager.Create(user3, Password);

            if (result3.Succeeded)
            {
                var mUser3 = new ModelUser(fName, sName, fEmail, Role, bDate, user3.Id);
                context.ModelUsers.Add(mUser3);
                context.SaveChanges();
            }
            //**//

            //Collect #1
            var newCollection = new Collect();
            newCollection.DateOfInitiative = DateTime.ParseExact("2017-09-12", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            newCollection.OwnerId = 1;
            newCollection.RecipientId = 2;
            newCollection.Name = "Zbiórka dla Aleksandra";
            newCollection.Amount = 100;
            context.Collections.Add(newCollection);
            context.SaveChanges();
            //**//

            //Collect #2
            var newCollection2 = new Collect();
            newCollection2.DateOfInitiative = DateTime.ParseExact("2017-09-24", "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            newCollection2.OwnerId = 2;
            newCollection2.RecipientId = 3;
            newCollection2.Name = "Zbiórka dla Michała";
            newCollection2.Amount = 200;
            context.Collections.Add(newCollection2);
            context.SaveChanges();
            //**//

            //Collect #1 _suffix
            //ModelUser modelUser = context.ModelUsers.Find(1);
            //var collect = modelUser.Collects.First();
            //collect.Name = collect.Name + "_suffix";
            //context.SaveChanges();
            //**//

            //CollectUser #1
            var newCollectionUsers = new CollectUser();
            newCollectionUsers.UserId = 1;
            newCollectionUsers.CollectId = 1;
            context.CollectionsUsers.Add(newCollectionUsers);
            context.SaveChanges();
            //**//

            //CollectUser #2
            var newCollectionUsers2 = new CollectUser();
            newCollectionUsers2.UserId = 2;
            newCollectionUsers2.CollectId = 2;
            context.CollectionsUsers.Add(newCollectionUsers2);
            context.SaveChanges();
            //**//

            //CollectGift #1
            var newCollectionGifts = new CollectGift();
            newCollectionGifts.UserId = 1;
            newCollectionGifts.CollectId = 1;
            newCollectionGifts.Name = "Kurtka dla Oli";
            context.CollectionsGifts.Add(newCollectionGifts);
            context.SaveChanges();
            //**//

            //CollectGiftRatings #1
            var newCollectionGiftRatings = new CollectGiftRating();
            newCollectionGiftRatings.UserId = 1;
            newCollectionGiftRatings.GiftId = 1;
            newCollectionGiftRatings.TheBestRating = true;
            context.CollectionsGiftRatings.Add(newCollectionGiftRatings);
            context.SaveChanges();
            //**//

            //Seed
            base.Seed(context);
        }
    }
}
