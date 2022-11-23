using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Message : IEntity
    {
        public int Id { get ; set ; }

        [Required(ErrorMessage = "You must enter a message subject.")]
        [MaxLength(75)]
        public string Subject { get ; set ; }

        [Required(ErrorMessage ="You must enter a message body.")]
        [MaxLength(500)]
        public string Body { get ; set ; }

        public DateTime Date { get ; set ; }

        public string SenderUsername { get; set; }

        public int SenderId { get; set; }

        public MessageBox Sender { get; set; }

        public ICollection<MessageMessageBox> Recipients { get; set; }
    }
}
