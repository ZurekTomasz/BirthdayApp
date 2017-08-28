using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AppModels;

namespace BirthdayApp.ViewModels
{
    public class CollectUserItem
    {
        public string UserName { get; set; }
        public bool GaveMoney { get; set; }
    }

    public class RadioGiftItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public bool Checked { get; set; }
    }

    public class CollectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int OwnerId { get; set; }
        public int RecipientId { get; set; }
        public string OwnerName { get; set; }
        public string RecipientName { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        [RegularExpression("^[0-9]{0,99999}$", ErrorMessage = "Value must be a natural number")]
        public decimal Amount { get; set; }
        public bool IsConfirmed { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfInitiative { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfAdd { get; set; }
        public RadioGiftItem Gift { get; set; }
        public List<RadioGiftItem> RadioGiftItems { get; set; }
        public List<CollectUserItem> Users { get; set; }
        public bool PossibilityEditCollectGift { get; set; }
        public string GiftName { get; set;}
        public decimal AmountPerPerson { get; set; }
    }
}