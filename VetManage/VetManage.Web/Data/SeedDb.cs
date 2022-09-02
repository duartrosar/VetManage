﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
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
                    Address = "Rua do Cocho"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder.");
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}