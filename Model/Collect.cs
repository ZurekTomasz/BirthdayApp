using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppModels
{
    public class Collect
    {
        public int Id { get; set; }
        [ForeignKey("Owner")]
        public int? OwnerId { get; set; }
        [ForeignKey("Recipient")]
        public int? RecipientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public virtual ModelUser Owner { get; set; }
        public virtual ModelUser Recipient { get; set; }
    }
}
