using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels
{
    public class CollectGiftRating
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        [ForeignKey("Collect")]
        public int? CollectId { get; set; }
        [ForeignKey("TheBestGift")]
        public int? TheBestGiftId { get; set; }

        public virtual User User { get; set; }
        public virtual Collect Collect { get; set; }
        public virtual CollectGift TheBestGift { get; set; }
    }
}
