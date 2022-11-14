using System.Collections.Generic;

namespace VetManage.Web.Data.Entities
{
    public class MessageBox : IEntity
    {
        public int Id { get ; set ; }

        public string Username { get ; set ; }

        public string UserId { get ; set ; }

        public User User { get ; set ; }

        public ICollection<Message> Inbox { get; set; }

        public ICollection<MessageMessageBox> Outbox { get; set; }
    }
}
