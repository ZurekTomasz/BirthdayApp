using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BirthdayApp.ViewModels
{
    public class CollectUsersListBoxViewModel
    {
        public List<SelectListItem> UsersList { get; set; }
        public int[] UsersListIds { get; set; }
        public int Selected { get; set; }
    }
}