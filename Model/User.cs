using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels
{
    public class ModelUser
    {
        public ModelUser()
        {
            this.Collects = new HashSet<Collect>();
            this.Collects2 = new HashSet<Collect>();

            this.CollectUsers = new HashSet<CollectUser>();
            
        }

        public ModelUser(string fName, string sName, string fEmail, string sRole, DateTime? bDate, string sEntityId)
        {
            DateTime zDate = bDate ?? default(DateTime);
            string sDate = zDate.ToString("yyyy-MM-dd");

            this.Name = fName + " " + sName;
            this.EntityId = sEntityId;
            this.Firstname = fName;
            this.Surname = sName;
            this.Email = fEmail;
            this.Role = sRole;
            this.DateOfBirth = bDate;
            this.DateOfAdd = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string EntityId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfAdd { get; set; }

        [InverseProperty("Owner")]
        public virtual ICollection<Collect> Collects { get; set; }
        [InverseProperty("Recipient")]
        public virtual ICollection<Collect> Collects2 { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<CollectUser> CollectUsers { get; set; }
        
    }
}
