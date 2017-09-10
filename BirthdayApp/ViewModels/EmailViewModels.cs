using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BirthdayApp.ViewModels
{
    public class EmailViewModels
    {
        public EmailViewModels() { }

        public EmailViewModels(string To, string Subject, string Body, string Email, string Password)
        {
            this.To = To;
            this.Subject = Subject;
            this.Body = Body;
            this.Email = Email;
            this.Password = Password;
        }

        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}