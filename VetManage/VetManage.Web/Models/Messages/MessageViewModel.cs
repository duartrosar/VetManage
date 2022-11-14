using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Messages
{
    public class MessageViewModel
    {
        public string SenderName { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required(ErrorMessage = "You must choose a recipient.")]
        public string[] Recipients { get; set; }

        public ICollection<MessageBox> RecipientsList { get; set; }
    }
}
