using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppModels;

namespace AppModels
{
    public class Collect
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public int RecipientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public virtual ModelUser Owner { get; set; }
    }
}
