using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Helpers;
using VetManage.Web.Models;

namespace VetManage.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        [Authorize]
        public IActionResult Index()
        {
            var users = _userHelper.GetAll()
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);

            AccountViewModel model = new AccountViewModel
            {
                Users = users.ToList(),
            };

            return View(model);
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Try to login
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    // If the user was trying to access an area
                    // That is only allowed to signed in users
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        // Redirect to the area the user wa trying to access
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Failed to login");

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Register()
        {
            return View();
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
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    if(result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The User could not be created.");

                        return View(model);
                    }

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
                        return RedirectToAction("Index", "Home");
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
                var user = await _userHelper.GetUserByIdAsync(model.Id);

                if(user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;

                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        ViewBag.UserMessage = "User Updated!";
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }

                }
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
