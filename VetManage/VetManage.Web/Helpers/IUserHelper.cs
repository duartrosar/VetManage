using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models;

namespace VetManage.Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
