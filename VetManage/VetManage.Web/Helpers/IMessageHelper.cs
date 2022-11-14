using System.Threading.Tasks;

namespace VetManage.Web.Helpers
{
    public interface IMessageHelper
    {
        Task InitializeMessageBox(string userId);
    }
}
