namespace VetManage.Web.Data.Entities
{
    public class MessageMessageBox
    {
        public int MessageId { get; set; }

        public int MessageBoxId { get; set; }

        public bool IsRead { get; set; }

        public Message Message { get; set; }

        public MessageBox MessageBox { get; set; }
    }
}
