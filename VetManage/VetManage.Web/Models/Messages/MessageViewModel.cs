using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Messages
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public string SenderName { get; set; }

        public int MessageBoxId { get; set; }

        [Required(ErrorMessage="You must enter a subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage="You must enter a enter a body")]
        public string Body { get; set; }

        public string BodyRaw { get; set; }

        public string DateString { get; set; }

        [Required(ErrorMessage = "You must choose a recipient.")]
        public string[] Recipients { get; set; }

        public bool IsRead { get; set; }

        public ICollection<MessageBox> RecipientsList { get; set; }
    }
}
