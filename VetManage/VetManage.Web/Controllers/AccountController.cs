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
using Vereyon.Web;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Account;

namespace VetManage.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IMailHelper _mailHelper;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IVetRepository _vetRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;

        public AccountController(
            IUserHelper userHelper,
            IConfiguration configuration,
            IMailHelper mailHelper,
            IOwnerRepository ownerRepository,
            IVetRepository vetRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _ownerRepository = ownerRepository;
            _vetRepository = vetRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if(user != null)
                {
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
                            return RedirectToAction("Dashboard", "Home");
                        }
                    }
                }
            }
            _flashMessage.Danger("The Username or Password are incorrect, please try again!");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userHelper.GetUserByIdAsync(userId);

            ViewData["Genders"] = _converterHelper.GetGenders();

            if (user != null)
            {
                //var entity = 
                var model = new EditProfileViewModel
                {
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MobileNumber = user.PhoneNumber,
                    ImageFullPath = user.ImageFullPath,
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    ImageId = user.ImageId,
                };

                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            ViewData["Genders"] = _converterHelper.GetGenders();

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the logged in user
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userHelper.GetUserByIdAsync(userId);


                    if (user != null)
                    {
                        // Check if the email introduced is already being used
                        var user2 = await _userHelper.GetUserByEmailAsync(model.Username);
                        
                        if (user2 != null && user.Email != model.Username)
                        {
                            _flashMessage.Danger("The email you introduced is already being used.");
                            return View(model);
                        }

                        Guid imageId = model.ImageId;

                        // Upload image if it's new
                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                        {
                            imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, user.BlobContainer);
                        }

                        model.ImageId = imageId;

                        // update the user's data
                        user = _converterHelper.EditProfileViewModelToUser(model, user, "users");

                        // Update the user 
                        var response = await _userHelper.UpdateUserAsync(user);

                        if (response.Succeeded)
                        {
                            _flashMessage.Confirmation("Profile updated!");

                            if (user.UserName != model.Username)
                            {
                                Response response2 = await SendConfirmNewEmailAsync(user, model);

                                if (response2.IsSuccess)
                                {
                                    _flashMessage.Confirmation("The instructions to confirm your new email have been sent.");
                                }
                            }

                            model.ImageFullPath = user.ImageFullPath;

                            return View(model);
                        }

                        _flashMessage.Danger("Your profile could not be updated, please try again.");

                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        [Route("Account/ConfirmNewEmail")]
        public async Task<IActionResult> ConfirmNewEmail(string userId, string newEmail,string token)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(newEmail) ||
                string.IsNullOrEmpty(token))
            {
                return new NotFoundViewResult("UserNotFound");
            }
            var user = await _userHelper.GetUserByIdAsync(userId);

            if(user != null)
            {
                var result = await _userHelper.ChangeEmailAsync(user, newEmail, token);

                if (result.Succeeded)
                {
                    user.UserName = newEmail;

                    await _userHelper.UpdateUserAsync(user);

                    ViewData["Message"] = "Your new email was confirmed.";

                    return View();
                }
            }
            ViewData["Message"] = "There was a problem confirming your new email, please try again.";

            return View();
        }

        public IActionResult ConfirmEmail(string userId, string confirmationToken, string passwordToken)
        {
            if(string.IsNullOrEmpty(userId) || 
                string.IsNullOrEmpty(confirmationToken) || 
                string.IsNullOrEmpty(passwordToken))
            {
                return RedirectToAction(nameof(NotAuthorized));
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
                    return new NotFoundViewResult("UserNotFound");
                }

                var result = await _userHelper.ConfirmEmailAsync(user, model.ConfirmationToken);

                if (!result.Succeeded)
                {
                    _flashMessage.Danger("There was a problem confirming your account, please try again.");

                    return View(model);
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
                            _flashMessage.Confirmation("Password changed and account confirmed. You may now login.");

                            return RedirectToAction(nameof(Login));
                        }
                    }
                    catch (Exception ex)
                    {
                        _flashMessage.Danger(ex.Message);
                    }
                }
            }
            _flashMessage.Danger("There was a problem confirming your account, please try again.");

            return View(model);
        }

        [Route("Account/RecoverPassword")]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/RecoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    _flashMessage.Danger("That email doesn't correspont to a registered user.");
                    return View(model);
                }

                Response response = await SendRecoverPasswordEmailAsync(user, model);

                if (response.IsSuccess)
                {
                    _flashMessage.Info("The instructions to recover your password have been sent to your email.");
                }

                return View(model);
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
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
                    _flashMessage.Confirmation("Password reset successfully!");
                    return View(model);
                }

                _flashMessage.Danger("Error while resetting the password.");

                return View(model);
            }

            _flashMessage.Danger("The user could not be found.");
            return View(model);
        }

        public IActionResult UserNotFound()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
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

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/GetUserAsync")]
        public async Task<JsonResult> GetUserAsync(int countryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user != null)
            {
                return Json(user);
            }
            return Json("NotAuthorized");
        }

        public async Task<Response> SendConfirmNewEmailAsync(User user, EditProfileViewModel model)
        {
            var myToken = await _userHelper.GenerateChangeEmailTokenAsync(user, model.Username);

            var link = this.Url.Action(
                "ConfirmNewEmail",
                "Account",
                new
                {
                    userId = user.Id,
                    newEmail = model.Username,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(model.Username, 
                "Confirm New Email", 
                $"<h1>Changed email</h1>" +
            $"To confirm your new email click in this link:</br></br>" +
            $"<a href = \"{link}\">Confirm new email</a>");

            return response;
        }

        
        public async Task<Response> SendRecoverPasswordEmailAsync(User user, RecoverPasswordViewModel model)
        {
            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

            var link = this.Url.Action(
                "ResetPassword",
                "Account",
                new { token = myToken }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(model.Email, 
                "Reset Password", 
                $"<h1>Password Reset</h1>" +
            $"To reset the password click in this link:</br></br>" +
            $"<a href = \"{link}\">Reset Password</a>");

            return response;
        }
    }
}
