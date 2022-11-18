using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VetManage.Web.Constants;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;

namespace VetManage.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IMessageHelper _messageHelper;
        private readonly IVetRepository _vetRepository;
        private readonly IConverterHelper _converterHelper;

        public SeedDb(
            DataContext context, 
            IUserHelper userHelper,
            IMessageHelper messageHelper,
            IVetRepository vetRepository,
            IConverterHelper converterHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _messageHelper = messageHelper;
            _vetRepository = vetRepository;
            _converterHelper = converterHelper;
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
                Vet vet = new Vet
                {
                    FirstName = "Duarte",
                    LastName = "Ribeiro",
                    MobileNumber = "214658973",
                    Address = "Rua do Cocho",
                    ImageId = new Guid(),
                    Gender = "Male",
                    DateOfBirth = new DateTime(1996,7,31)
                };

                user = _converterHelper.ToUser(vet, new User());

                user.PasswordChanged = true;
                user.Email = "duartrosar@gmail.com";
                user.UserName = "duartrosar@gmail.com";
                user.ImageFullPath = "https://vetmanage.azurewebsites.net/images/nouser.png";

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder.");
                }

                await _userHelper.AddUserToRoleAsync(user, Roles.Admin.ToString());

                await _userHelper.AddUserToRoleAsync(user, Roles.Employee.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                await _userHelper.ConfirmEmailAsync(user, token);

                vet.User = user;

                await _vetRepository.CreateAsync(vet);

                await _context.SaveChangesAsync();

                await _messageHelper.InitializeMessageBox(user.Id);
            }
        }
    }
}
