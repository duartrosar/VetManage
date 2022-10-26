using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VetManage.Web.Constants;
using VetManage.Web.Data.Entities;
using VetManage.Web.Helpers;

namespace VetManage.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync(Roles.Admin.ToString());
            await _userHelper.CheckRoleAsync(Roles.Employee.ToString());
            await _userHelper.CheckRoleAsync(Roles.Client.ToString());

            await AddAdminUser();
        }

        private async Task AddAdminUser()
        {
            var user = await _userHelper.GetUserByEmailAsync("duartrosar@gmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Duarte",
                    LastName = "Ribeiro",
                    Email = "duartrosar@gmail.com",
                    UserName = "duartrosar@gmail.com",
                    PhoneNumber = "214658973",
                    Address = "Rua do Cocho",
                    PasswordChanged = true,
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder.");
                }

                await _userHelper.AddUserToRoleAsync(user, Roles.Admin.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                await _userHelper.ConfirmEmailAsync(user, token);

                await _context.SaveChangesAsync();
            }
        }
    }
}
