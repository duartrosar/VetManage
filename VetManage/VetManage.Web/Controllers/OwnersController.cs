using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Owners;

namespace VetManage.Web.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class OwnersController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMessageBoxRepository _messageBoxRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IMessageHelper _messageHelper;
        private readonly IFlashMessage _flashMessage;

        public OwnersController(
            IOwnerRepository ownerRepository,
            IMessageBoxRepository messageBoxRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IMessageHelper messageHelper,
            IFlashMessage flashMessage)
        {
            _ownerRepository = ownerRepository;
            _messageBoxRepository = messageBoxRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _messageHelper = messageHelper;
            _flashMessage = flashMessage;
        }

        // GET: Owners
        //[Route("Owners/Index")]
        public IActionResult Index()
        {
            return View(_ownerRepository
                .GetAll()
                .OrderBy(o => o.FirstName)
                .ThenBy(o => o.LastName));
        }

        // GET: Vets/Create
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            var owner = await _ownerRepository.GetWithUserByIdAsync(id.Value);

            if (owner == null)
            {
                // owner not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            var model = new OwnerDetailsViewModel
            {
                OwnerViewModel = _converterHelper.ToOwnerViewModel(owner),
                Username = owner.User.UserName,
            };

            return View(model);
        }

        // GET: Vets/Create
        public IActionResult Create()
        {
            var model = new RegisterOwnerViewModel
            {
                OwnerViewModel = new OwnerViewModel
                {
                    DateOfBirth = DateTime.Now,
                },
            };

            ViewData["Genders"] = _converterHelper.GetGenders();

            return View(model);
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterOwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = Guid.Empty;

                    if (model.OwnerViewModel.ImageFile != null && model.OwnerViewModel.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.OwnerViewModel.ImageFile, "owners");
                    }

                    var user = await _userHelper.GetUserByEmailAsync(model.Username);

                    if (user == null)
                    {
                        // Convert Owner
                        var owner = _converterHelper.ToOwner(model.OwnerViewModel, true, imageId);

                        user = _converterHelper.ToUser(owner, new User(), "owners");

                        user.Email = model.Username;
                        user.UserName = model.Username;
                        user.PasswordChanged = false;

                        // Generate random password
                        var password = Guid.NewGuid().ToString();

                        var result = await _userHelper.AddUserAsync(user, password);

                        if (result != IdentityResult.Success)
                        {
                            throw new Exception("The user could not be created, please try again.");
                        }

                        // Add role or roles to user
                        await _userHelper.AddUserToRoleAsync(user, "Client");

                        // get the newly created user and set it as the owners's user
                        owner.User = await _userHelper.GetUserByEmailAsync(model.Username);

                        // Save Owner
                        await _ownerRepository.CreateAsync(owner);

                        // Create user's MessageBox
                        await _messageHelper.InitializeMessageBox(user.Id);

                        Response response = await ConfirmEmailAsync(user, model);

                        if (response.IsSuccess)
                        {
                            _flashMessage.Confirmation("Owner has been created and confirmation email has been sent to user.");
                        }

                        model.OwnerViewModel.ImageId = imageId;

                        return RedirectToAction(nameof(Index));
                    }

                    _flashMessage.Danger("That email is already being used by another user.");

                    ViewData["Genders"] = _converterHelper.GetGenders();

                    return View(model);
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            var owner = await _ownerRepository.GetWithUserByIdAsync(id.Value);

            if (owner == null)
            {
                // owner not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            var model = _converterHelper.ToOwnerViewModel(owner);

            ViewData["Genders"] = _converterHelper.GetGenders();

            return View(model);
        }

        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewData["Genders"] = _converterHelper.GetGenders();

                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "owners");
                    }

                    // Get the user related with the owner
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);

                    if(user == null)
                    {
                        return new NotFoundViewResult("OwnerNotFound");
                    }

                    var owner = _converterHelper.ToOwner(model, false, imageId);

                    user = _converterHelper.ToUser(owner, user, "owners");

                    model.ImageId = imageId;

                    // Update the user 
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        model.User = user;

                        owner.UserId = user.Id;

                        await _ownerRepository.UpdateAsync(owner);

                        _flashMessage.Confirmation("Owner has been updated.");

                        return View(model);
                    }

                    _flashMessage.Danger("An error ocurred whilst tryng to update the owner, please try again.");

                    return View(model);
                }
                catch (Exception ex)
                {
                    if (!await _ownerRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("OwnerNotFound");
                    }
                    else
                    {
                        _flashMessage.Danger(ex.Message);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                // owner not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            var owner = await _ownerRepository.GetWithUserByIdAsync(id.Value);

            if(owner == null || owner.User == null)
            {
                // owner not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            try
            {
                var user = await _userHelper.GetUserByIdAsync(owner.User.Id);

                await _userHelper.DeleteUserAsync(user);

                _flashMessage.Confirmation("Owner deleted successfully");

                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"You can't delete {owner.FullName}. Too much depends on it";
                    ViewBag.ErrorMessage = $"You can't delete this owner because there are pets and messages associated with it.</br></br>" +
                        $"Try to delete all pets associated with this user and try again.</br></br>" +
                        $"Note: If there are messages associated with this user you may not delete it."; 
                }

                return View("Error");
            }
        }

        [HttpPost]
        [Route("Owners/GetUserAsync")]
        public async Task<JsonResult> GetUserAsync(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            return Json(user);
        }

        public IActionResult OwnerNotFound()
        {
            return View();
        }

        public async Task<Response> ConfirmEmailAsync(User user, RegisterOwnerViewModel model)
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
                model.Username,
                "Activate Account",
                $"<h1>Email Confirmation</h1>" +
                $"To activate your account please click the link and set up a new password:</br></br><a href = \"{tokenLink}\">Confirm Account</a>");

            return response;
        }
    }
}
