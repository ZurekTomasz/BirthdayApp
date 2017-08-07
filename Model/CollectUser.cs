using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels
{
    public class CollectUser
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        [ForeignKey("Collect")]
        public int? CollectId { get; set; }
        public bool GaveMoney { get; set; }
        public virtual ModelUser User { get; set; }
        public virtual Collect Collect { get; set; }
    }
}
