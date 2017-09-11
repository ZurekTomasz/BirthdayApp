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
    public class User
    {
        public User()
        {
            this.IsActive = true;
        }

        public User(string fName, string sName, string fEmail, string sRole, DateTime? bDate, string sEntityId)
        {
            this.Name = fName + " " + sName;
            this.EntityId = sEntityId;
            this.Firstname = fName;
            this.Surname = sName;
            this.Email = fEmail;
            this.Role = sRole;
            this.DateOfBirth = bDate;
            this.DateOfAdd = DateTime.Now;
            this.IsActive = true;
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
        public bool IgnoreEmailCreate { get; set; }
        public bool IgnoreEmailConfirm { get; set; }
        public bool IsActive { get; set; }
    }
}
