using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;
using VetManage.Web.Data.Entities;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Account;
using VetManage.Web.Models.Users;

namespace VetManage.Web.Controllers
{
    /// <summary>
    /// From the users perspective, this is the "Employees" controller since they can 
    /// only CRUD the users that are a part of the Employee Role from this controller
    /// </summary>
    [Authorize(Roles = "Admin")] // TO DO: (Role = "Admin")
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMessageHelper _messageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IMailHelper _mailHelper;

        public UsersController(
            IUserHelper userHelper,
            IMessageHelper messageHelper,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IFlashMessage flashMessage,
            IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _messageHelper = messageHelper;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            // We only want to manage the "Employee" users so that's what we get
            var employeeUsers = await _userHelper.GetUsersInRoleAsync("Employee");

            return View(employeeUsers
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName));
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userHelper.GetUserByIdAsync(id);

            if (user == null)
            {
                // vet not found
                return new NotFoundViewResult("UserNotFound");
            }

            var model = _converterHelper.ToUserViewModel(user);

            model.Id = id;

            if (await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                model.IsAdmin = true;
            }

            return View(model);
        }

        public IActionResult Create()
        {
            var model = new UserViewModel
            {
                DateOfBirth = DateTime.Now,
            };

            ViewData["Genders"] = _converterHelper.GetGenders();

            return View(model);
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            ViewData["Genders"] = _converterHelper.GetGenders();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userHelper.GetUserByEmailAsync(model.Username);

                    if(user == null)
                    {
                        Guid imageId = model.ImageId;

                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                        {
                            imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                        }

                        user = _converterHelper.ToUser(new User(), model, true, "users");

                        user.Email = model.Username;
                        user.PasswordChanged = false;

                        // Generate random password
                        var password = Guid.NewGuid().ToString();

                        var result = await _userHelper.AddUserAsync(user, password);

                        if (result != IdentityResult.Success)
                        {
                            throw new Exception("The user could not be created, please try again.");
                        }

                        // Add role or roles to user
                        await _userHelper.AddUserToRoleAsync(user, "Employee");

                        if (model.IsAdmin)
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Admin");
                        }

                        // Create user's MessageBox
                        await _messageHelper.InitializeMessageBox(user.Id);

                        Response response = await ConfirmEmailAsync(user);

                        if (response.IsSuccess)
                        {
                            _flashMessage.Confirmation("Employee has been created and confirmation email has been sent to user.");
                        }

                        model.ImageId = imageId;

                        return RedirectToAction(nameof(Index));
                    }

                    _flashMessage.Danger("That email is already being used by another user, please try again");

                    return View(model);
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        // GET: Vets/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userHelper.GetUserByIdAsync(id);

            if (user == null)
            {
                // vet not found
                return new NotFoundViewResult("UserNotFound");
            }

            var model = _converterHelper.ToUserViewModel(user);

            if (await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                model.IsAdmin = true;
            }

            ViewData["Genders"] = _converterHelper.GetGenders();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            ViewData["Genders"] = _converterHelper.GetGenders();

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the user being edited so we can check if the email has changed
                    var user = await _userHelper.GetUserByIdAsync(model.Id);

                    if(user != null)
                    {
                        // Try to get an user with the new email introduced to check if that email is already being used
                        var user2 = await _userHelper.GetUserByIdAsync(model.Username);

                        if(user2 != null && user.Email != model.Username)
                        {
                            _flashMessage.Danger("The email you introduced is already being used.");

                            return View(model);
                        }

                        Guid imageId = model.ImageId;

                        if (model.ImageFile != null && model.ImageFile.Length > 0)
                        {
                            imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, model.BlobContainer);
                        }

                        model.ImageId = imageId;

                        user = _converterHelper.ToUser(user, model, false, model.BlobContainer);

                        var response = await _userHelper.UpdateUserAsync(user);

                        if (response.Succeeded)
                        {
                            _flashMessage.Confirmation("User updated!");

                            if (model.IsAdmin)
                            {
                                await _userHelper.AddUserToRoleAsync(user, "Admin");
                            } else
                            {
                                await _userHelper.RemoveUserFromRoleAsync(user, "Admin");
                            }

                            if (user.Email != model.Username)
                            {
                                Response response2 = await SendConfirmNewEmailAsync(user, model);
                                if (response2.IsSuccess)
                                {
                                    _flashMessage.Confirmation("The instructions to confirm the new email have been sent.");
                                }
                            }

                            return View(model);
                        }
                    }

                    _flashMessage.Danger("The user could not be updated, please try again.");

                    return View(model);
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            var user = await _userHelper.GetUserByIdAsync(id);

            if(user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }

            try
            {
                await _userHelper.DeleteUserAsync(user);

                _flashMessage.Confirmation("User deleted successfully");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"You can't delete {user.FullName}. Too much depends on them.";
                    ViewBag.ErrorMessage = $"You can't delete this user because there are messages or appointments associated with it.</br></br>" +
                        $"Delete all appointments associated with this user and try again.</br></br>" +
                        $"Note: If there are messages associated with this user you may not be able to delete it.";
                }

                return View("Error");
            }
        }

        public IActionResult UserNotFound()
        {
            return View();
        }

        public async Task<Response> SendConfirmNewEmailAsync(User user, UserViewModel model)
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

        private async Task<Response> ConfirmEmailAsync(User user)
        {
            string confirmationToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            string passwordToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            string tokenLink = Url.Action(
                "ConfirmEmail",
                "Account", new
                {
                    userId = user.Id,
                    confirmationToken = confirmationToken,
                    passwordToken = passwordToken

                }, protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(
                user.UserName,
                "Activate Account",
                $"<h1>Email Confirmation</h1>" +
                $"To activate your account please click the link and set up a new password:</br></br><a href = \"{tokenLink}\">Confirm Account</a>");

            return response;
        }
    }
}
