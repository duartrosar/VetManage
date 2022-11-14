using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Message : IEntity
    {
        public int Id { get ; set ; }

        [Required]
        public string Subject { get ; set ; }

        [Required]
        public string Body { get ; set ; }

        public string SenderUsername { get; set; }

        public int SenderId { get; set; }

        public MessageBox Sender { get; set; }

        public ICollection<MessageMessageBox> Recipients { get; set; }
    }
}
