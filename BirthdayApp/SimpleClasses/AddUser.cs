using System;
using System.Globalization;
using System.Linq;
using BirthdayApp.Models;
using AppModels;

namespace BirthdayApp.SimpleClasses
{
    public class AddUser
    {

        public void AddModelUser(string fName, string sName, string fEmail, string Role, string bDate, string EntityId)
        {
            try
            {
                var newModelUser = new ModelUser();

                newModelUser.Firstname = fName;
                newModelUser.Surname = sName;
                newModelUser.EntityId = EntityId;
                newModelUser.Email = fEmail;
                newModelUser.Role = Role;
                newModelUser.DateOfBirth = DateTime.ParseExact(bDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                newModelUser.DateOfAdd = DateTime.Now;

                using (var context = new ApplicationDbContext())
                {
                    context.ModelUsers.Add(newModelUser);
                    context.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                string sEx = Ex.ToString();
            }
        }

    }
}