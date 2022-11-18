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

        public AccountController(
            IUserHelper userHelper,
            IConfiguration configuration,
            IMailHelper mailHelper,
            IOwnerRepository ownerRepository,
            IVetRepository vetRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _mailHelper = mailHelper;
            _ownerRepository = ownerRepository;
            _vetRepository = vetRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
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

                if(user == null)
                {
                    return View(model);
                }
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

        public async Task<IActionResult> EditProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userHelper.GetUserByIdAsync(userId);

            if(user != null)
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
                };

                if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
                {
                    var entity = await _vetRepository.GetByUserIdAsync(user);

                    model.DateOfBirth = entity.DateOfBirth;
                    model.Gender = entity.Gender;
                    model.ImageId = entity.ImageId;
                } else if(await _userHelper.IsUserInRoleAsync(user, "Client"))
                {
                    var entity = await _ownerRepository.GetByUserIdAsync(user);

                    model.DateOfBirth = entity.DateOfBirth;
                    model.Gender = entity.Gender;
                    model.ImageId = entity.ImageId;
                }
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userHelper.GetUserByIdAsync(userId);

                    var user2 = await _userHelper.GetUserByEmailAsync(model.Username);

                    if (user2 != null && user.UserName != model.Username)
                    {
                        ViewBag.Message = "The email you introduced is already being used.";
                        return View(model);
                    }

                    if (user != null)
                    {
                        if (await _userHelper.IsUserInRoleAsync(user, "Employee"))
                        {
                            var vet = await _vetRepository.GetByUserIdAsync(user);

                            // Upload image if it's new
                            Guid imageId = model.ImageId;

                            if (model.ImageFile != null && model.ImageFile.Length > 0)
                            {
                                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "vets");
                                model.ImageId = imageId;
                            }

                            vet = _converterHelper.EditProfileViewModelToVet(model, vet, imageId);
                            model.ImageFullPath = vet.ImageFullPath;
                            user = _converterHelper.ToUser(vet, user);

                            await _vetRepository.UpdateAsync(vet);

                            // Update the user 
                            var response = await _userHelper.UpdateUserAsync(user);

                            if (response.Succeeded)
                            {
                                await _vetRepository.UpdateAsync(vet);

                                if (user.UserName != model.Username)
                                {
                                    Response response2 = await SendConfirmNewEmailAsync(user, model);

                                    if (response2.IsSuccess)
                                    {
                                        ViewBag.Message = "The instructions to confirm your new email have been sent.";
                                    }
                                }
                                return View(model);
                            }
                        }
                        else if (await _userHelper.IsUserInRoleAsync(user, "Client"))
                        {
                            var owner = await _ownerRepository.GetByUserIdAsync(user);

                            // Upload image if it's new
                            Guid imageId = model.ImageId;

                            if (model.ImageFile != null && model.ImageFile.Length > 0)
                            {
                                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "owners");
                                model.ImageId = imageId;
                            }

                            owner = _converterHelper.EditProfileViewModelToOwner(model, owner, imageId);

                            model.ImageFullPath = owner.ImageFullPath;

                            user = _converterHelper.ToUser(owner, user);

                            // Update the user 
                            var response = await _userHelper.UpdateUserAsync(user);

                            if (response.Succeeded)
                            {
                                await _ownerRepository.UpdateAsync(owner);

                                if (user.UserName != model.Username)
                                {
                                    Response response2 = await SendConfirmNewEmailAsync(user, model);

                                    if (response2.IsSuccess)
                                    {
                                        ViewBag.Message = "The instructions to confirm your new email have been sent.";
                                    }
                                }
                                return View(model);
                            }
                        }

                        return View(model);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
           
            return View();
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
                    // TODO: Succeded
                    return View();
                }
            }
            // TODO: User Not Found
            return View();
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
                    ModelState.AddModelError(string.Empty, "The email doesn't correspont to a registered user.");
                    return View(model);
                }

                Response response = await SendRecoverPasswordEmailAsync(user, model);

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password has been sent to email.";
                }

                return View();

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
                    this.ViewBag.Message = "Password reset successfull";
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

        [HttpPost]
        [Route("Account/GetUserAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user != null)
            {
                //user.R
                return Json(user);
            }
            return Json("NotAuthorized");
        }

        public IActionResult UserNotFound()
        {
            return View();
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

            Response response = _mailHelper.SendEmail(model.Username, "Shop Password Reset", $"<h1>Changed email</h1>" +
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

            Response response = _mailHelper.SendEmail(model.Email, "Shop Password Reset", $"<h1>Password Reset</h1>" +
            $"To reset the password click in this link:</br></br>" +
            $"<a href = \"{link}\">Reset Password</a>");

            return response;
        }
    }
}
