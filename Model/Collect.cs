using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Model
{
    public class xCollect
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        //public virtual User Owner { get; set; }

        //public virtual ApplicationUser Owner { get; set; }

        //public virtual Course Course { get; set; }
        //public virtual Student Student { get; set; }
    }
}
