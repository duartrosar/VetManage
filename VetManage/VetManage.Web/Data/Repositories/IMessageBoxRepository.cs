using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface IMessageBoxRepository : IGenericRepository<MessageBox>
    {
        Task<MessageBox> GetMessageBoxByUserIdAsync(string userId);

        Task SendMessage(Message message, List<MessageBox> recipients);

        Task<IQueryable<MessageBox>> GetRecipientsAsync(int messageBoxId);

        IQueryable<MessageBox> GetSelectedRecipients(int[] ids);

        Task<IQueryable<Message>> GetInboxByUserId(string userId);

        Task<IQueryable<Message>> GetOutboxByUserId(string userId);

        Task<IQueryable<MessageMessageBox>> GetMessageMessageBoxByUserId(string userId);

        Task<MessageMessageBox> GetMessageMessageBox(int messageId, int messageBoxId);

        Task ReadMessage(MessageMessageBox messageMessageBox);

        Task<Message> GetMessageById(int messageId);

        Task<IQueryable> GetUnreadMessages(string userId);
    }
}
