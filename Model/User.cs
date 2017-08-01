using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels
{
    public class ModelUser
    {
        public int Id { get; set; }
        public string EntityId { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfAdd { get; set; }
    }
}
