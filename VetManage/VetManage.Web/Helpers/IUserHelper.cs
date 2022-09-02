using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);
    }
}
