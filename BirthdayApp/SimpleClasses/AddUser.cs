using System;
using System.Globalization;
using System.Linq;
using BirthdayApp.Models;
using AppModels;

namespace BirthdayApp.SimpleClasses
{
    public class AddUser
    {

       // public void AddModelUser(string fName, string sName, string fEmail, string sRole, string bDate, string sEntityId)
       // {
       //     try
       //     {
       //         var newModelUser = new ModelUser();

       //         newModelUser.Firstname = fName;
       //         newModelUser.Surname = sName;
       //         newModelUser.EntityId = sEntityId;
       //         newModelUser.Email = fEmail;
       //         newModelUser.Role = sRole;
       //         newModelUser.DateOfBirth = DateTime.ParseExact(bDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
       //         newModelUser.DateOfAdd = DateTime.Now;

       //         using (var context = new ApplicationDbContext())
       //         {
       //             context.ModelUsers.Add(newModelUser);
       //             context.SaveChanges();
       //         }
       //     }
       //     catch (Exception Ex)
       //     {
       //         string sEx = Ex.ToString();
       //     }
       //}

    }
}