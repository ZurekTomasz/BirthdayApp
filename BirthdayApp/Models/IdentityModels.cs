using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AppModels;

namespace BirthdayApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        //public string FirstName { get; set; }
        //public string Surname { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<ModelUser> ModelUsers { get; set; }
        public DbSet<Collect> Collections { get; set; }



        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Collect>()
        //                .HasRequired(m => m.Owner)
        //                .WithMany(t => t.Collects)
        //                .HasForeignKey(m => m.OwnerId)
        //                .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<Collect>()
        //                .HasRequired(m => m.Recipient)
        //                .WithMany(t => t.Collects2)
        //                .HasForeignKey(m => m.RecipientId)
        //                .WillCascadeOnDelete(false);
        //}


    }
}