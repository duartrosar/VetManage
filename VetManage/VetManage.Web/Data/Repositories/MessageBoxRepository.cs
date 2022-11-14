using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Helpers;

namespace VetManage.Web.Data.Repositories
{
    public class MessageBoxRepository : GenericRepository<MessageBox>, IMessageBoxRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public MessageBoxRepository(
            DataContext context,
            IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task<MessageBox> GetMessageBoxByUserIdAsync(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            if(user == null)
            {
                return null;
            }

            return await _context.MessageBoxes.FirstOrDefaultAsync(mb => mb.UserId == userId);
        }

        public async Task<IQueryable<MessageBox>> GetRecipientsAsync(int messageBoxId)
        {
            if (await ExistsAsync(messageBoxId))
            {
                return _context.MessageBoxes.
                    Where(mb => mb.Id != messageBoxId);
            }
            return null;
        }

        public IQueryable<MessageBox> GetSelectedRecipients(int[] ids)
        {
            return _context.MessageBoxes.Where(mb => ids.Contains(mb.Id));
        }

        public async Task SendMessage(Message message, List<MessageBox> recipients)
        {
            foreach(var recipient in recipients)
            {
                await _context.MessageMessageBox.AddAsync(new MessageMessageBox
                {
                    Message = message,
                    MessageBox = recipient,
                    IsRead = false
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<Message>> GetInboxByUserId(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            if(user != null)
            {
                var messageBox = await _context.MessageBoxes.FirstOrDefaultAsync(mb => mb.UserId == userId);

                return _context.Messages
                    .Where(m => m.Recipients.Any(r => r.MessageBoxId == messageBox.Id))
                    .Include(m => m.Sender)
                    .ThenInclude(mb => mb.User);
            }

            return null;
        }

        public async Task<IQueryable<Message>> GetOutboxByUserId(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user != null)
            {
                var messageBox = await _context.MessageBoxes.FirstOrDefaultAsync(mb => mb.UserId == userId);

                return _context.Messages.Where(m => m.SenderId == messageBox.Id);
            }

            return null;
        }
    }
}
