using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BirthdayApp.Models
{
    public class Collect
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

}