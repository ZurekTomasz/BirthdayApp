using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels
{
    public class CollectGift
    {
        public CollectGift()
        {
            this.CollectGiftRatings2 = new HashSet<CollectGiftRating>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        [ForeignKey("Collect")]
        public int? CollectId { get; set; }
        public int Rating { get; set; }
        public virtual ModelUser User { get; set; }
        public virtual Collect Collect { get; set; }

        [InverseProperty("Gift")]
        public virtual ICollection<CollectGiftRating> CollectGiftRatings2 { get; set; }
    }
}
