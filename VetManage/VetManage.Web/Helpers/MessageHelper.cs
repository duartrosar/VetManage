using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;

namespace VetManage.Web.Helpers
{
    public class MessageHelper : IMessageHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly IMessageBoxRepository _messageBoxRepository;

        public MessageHelper(
            IUserHelper userHelper,
            IMessageBoxRepository messageBoxRepository)
        {
            _userHelper = userHelper;
            _messageBoxRepository = messageBoxRepository;
        }

        public async Task InitializeMessageBox(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            if(user != null)
            {
                MessageBox messageBox = new MessageBox
                {
                    UserId = user.Id,
                    Username = user.UserName,
                };

                await _messageBoxRepository.CreateAsync(messageBox);
            }
        }
    }
}
