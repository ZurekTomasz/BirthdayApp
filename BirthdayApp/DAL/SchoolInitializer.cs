using BirthdayApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BirthdayApp.DAL
{
    public class SchoolInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {

        }
    }
}