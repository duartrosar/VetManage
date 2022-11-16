using System.Collections.Generic;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Helpers
{
    public interface IMessageHelper
    {
        Task InitializeMessageBox(string userId);
    }
}
