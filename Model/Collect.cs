using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppModels
{
    public class Collect
    {
        public Collect()
        {
            this.Users = new HashSet<CollectUser>();
            this.Gifts = new HashSet<CollectGift>();

            this.DateOfAdd = DateTime.Now;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("Owner")]
        public int? OwnerId { get; set; }
        [ForeignKey("Recipient")]
        public int? RecipientId { get; set; }
        public string Description { get; set; }
        public bool IsConfirmed { get; set; }
        public int Amount { get; set; }
        [Required]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfInitiative { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfAdd { get; set; }
        public bool IsActive { get; set; }

        public virtual User Owner { get; set; }
        public virtual User Recipient { get; set; }

        [InverseProperty("Collect")]
        public virtual ICollection<CollectUser> Users { get; set; }
        [InverseProperty("Collect")]
        public virtual ICollection<CollectGift> Gifts { get; set; }
    }
}
