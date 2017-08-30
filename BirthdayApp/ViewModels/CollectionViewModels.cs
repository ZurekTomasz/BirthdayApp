using AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BirthdayApp.ViewModels
{
    [DebuggerDisplay("Name: {Name}")]
    public class CollectListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public int RecipientId { get; set; }
        public string OwnerName { get; set; }
        public string RecipientName { get; set; }
        public string Description { get; set; }
        public bool IsConfirmed { get; set; }
        public int Amount { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfInitiative { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfAdd { get; set; }
        public bool YoureInCollection { get; set; }
    }
}