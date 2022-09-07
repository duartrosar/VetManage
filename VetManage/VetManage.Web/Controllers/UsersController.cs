using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Helpers;
using VetManage.Web.Models;

namespace VetManage.Web.Controllers
{
    [Authorize] // TO DO: (Role = "Admin")
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;

        public UsersController(
            IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            var users = _userHelper.GetAll()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);

            var roles = _userHelper.GetComboRoles();
            
            RegisterNewUserViewModel registerViewModel = new RegisterNewUserViewModel()
            {
                Roles = roles,
            };

            EditUserViewModel editViewModel = new EditUserViewModel()
            {
                Roles = roles,
            };

            AccountViewModel model = new AccountViewModel
            {
                Users = users.ToList(),
                RegisterNewUser = registerViewModel,
                EditUser = editViewModel,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists and get it if it does
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    // Add new user with the data from the model
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        RoleId = model.RoleId,
                        RoleName = model.RoleName,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The User could not be created.");

                        return View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, model.RoleName);

                    // Login the newly created user
                    LoginViewModel loginViewModel = new LoginViewModel
                    {
                        Username = user.UserName,
                        Password = model.Password,
                        RememberMe = false,
                    };

                    // Try to login
                    var result2 = await _userHelper.LoginAsync(loginViewModel);

                    if (result2.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if user exists
                var user = await _userHelper.GetUserByIdAsync(model.Id);

                if (user != null)
                {
                    // update user's data
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.RoleId = model.RoleId;
                    user.RoleName = model.RoleName;

                    // Update user in database
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User Updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }

                    if(model.RoleName != user.RoleName)
                    {
                        await _userHelper.RemoveUserFromRoleAsync(user, user.RoleName);
                        await _userHelper.AddUserToRoleAsync(user, model.RoleName);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditUserViewModel model)
        {
            var user = await _userHelper.GetUserByIdAsync(model.Id);

            try
            {
                await _userHelper.DeleteUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{user.FullName} could not be deleted.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
