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
            this.CollectUsers2 = new HashSet<CollectUser>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Owner")]
        public int? OwnerId { get; set; }
        [ForeignKey("Recipient")]
        public int? RecipientId { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        [RegularExpression("^[0-9]{0,99999}$", ErrorMessage = "Value must be a natural number")]
        public int Amount { get; set; }
        public virtual ModelUser Owner { get; set; }
        public virtual ModelUser Recipient { get; set; }

        [InverseProperty("Collect")]
        public virtual ICollection<CollectUser> CollectUsers2 { get; set; }
    }
}
