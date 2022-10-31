using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Account;

namespace VetManage.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;

        public AccountController(
            IUserHelper userHelper,
            IConfiguration configuration,
            IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
        }

        //[Authorize]
        //public IActionResult Index()
        //{
        //    var users = _userHelper.GetAll()
        //        .OrderBy(u => u.FirstName)
        //        .ThenBy(u => u.LastName);

        //    AccountViewModel model = new AccountViewModel
        //    {
        //        Users = users.ToList(),
        //    };

        //    return View(model);
        //}

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
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user.PasswordChanged)
                {
                    // Try to login
                    var result = await _userHelper.LoginAsync(model);

                    if (result.Succeeded)
                    {
                        // If the user was trying to access an area
                        // That is only allowed to signed in users
                        if (Request.Query.Keys.Contains("ReturnUrl"))
                        {
                            // Redirect to the area the user was trying to access
                            return Redirect(Request.Query["ReturnUrl"].First());
                        }
                        return RedirectToAction("Index", "Home");
                    }
                } else
                {
                    // TODO: you haven't changed your password yet
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


        public async Task<IActionResult> ConfirmEmail(string userId, string confirmationToken, string passwordToken)
        {
            if(string.IsNullOrEmpty(userId) || 
                string.IsNullOrEmpty(confirmationToken) || 
                string.IsNullOrEmpty(passwordToken))
            {
                return NotFound();
            }

            var model = new ConfirmAccountViewModel()
            {
                UserId = userId,
                ConfirmationToken = confirmationToken,
                PasswordToken = passwordToken,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userHelper.ConfirmEmailAsync(user, model.ConfirmationToken);

                if (!result.Succeeded)
                {
                    return NotFound();
                }

                var result2 = await _userHelper.ResetPasswordAsync(user, model.PasswordToken, model.Password);

                if (result2.Succeeded)
                {
                    try
                    {
                        // User has changed its password after its account was first created
                        user.PasswordChanged = true;

                        // Update the user 
                        var response = await _userHelper.UpdateUserAsync(user);

                        if (response.Succeeded)
                        {
                            ViewBag.Message = "Password changed and account confirmed. You may now login.";
                            return RedirectToAction(nameof(Login));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username);

            if(user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    this.ViewBag.Nessage = "Password reset successfull";
                    return RedirectToAction(nameof(Login));
                }

                ViewBag.Message = "Error while resetting the password.";

                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public IActionResult NotAuthorized()
        {

            return View();
        }
    }
}
