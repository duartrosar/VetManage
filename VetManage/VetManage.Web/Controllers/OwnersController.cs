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

        public OwnersController(
            IOwnerRepository ownerRepository,
            IMessageBoxRepository messageBoxRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IMessageHelper messageHelper)
        {
            _ownerRepository = ownerRepository;
            _messageBoxRepository = messageBoxRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _messageHelper = messageHelper;
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
            return View();
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
                        // Create new user
                        user = new User
                        {
                            FirstName = model.OwnerViewModel.FirstName,
                            LastName = model.OwnerViewModel.LastName,
                            Email = model.Username,
                            UserName = model.Username,
                            Address = model.OwnerViewModel.Address,
                            PhoneNumber = model.OwnerViewModel.MobileNumber,
                            PasswordChanged = false,
                        };

                        // Generate random password
                        var password = Guid.NewGuid().ToString();

                        var result = await _userHelper.AddUserAsync(user, password);

                        if (result != IdentityResult.Success)
                        {
                            ModelState.AddModelError(string.Empty, "The User could not be created.");

                            return View(model);
                        }

                        // Add role or roles to user
                        await _userHelper.AddUserToRoleAsync(user, "Employee");

                        // get the newly created user and set it as the vet's user
                        model.OwnerViewModel.User = await _userHelper.GetUserByEmailAsync(model.Username);

                        // Create Owner
                        var owner = _converterHelper.ToOwner(model.OwnerViewModel, true, imageId);

                        await _ownerRepository.CreateAsync(owner);

                        // Create user's MessageBox
                        await _messageHelper.InitializeMessageBox(user.Id);

                        // Send confirmation and change password email
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
                            "Email Confirmation",
                            $"<h1>Email Confirmation</h1>" +
                            $"To activate your account please click the link and set up a new password:</br></br><a href = \"{tokenLink}\">Confirm Account</a>");

                        if (response.IsSuccess)
                        {
                            ViewBag.Message = "Confirmation email has been sent";
                            return RedirectToAction(nameof(Index));
                        }

                        return RedirectToAction(nameof(Index));
                        // TODO: Owner could not be created
                    } else
                    {
                        // TODO: Owner already exists
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return RedirectToAction(nameof(Index));
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

            return View(model);
        }

        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "owners");
                    }

                    // Get the user the user chose from the dropdown with the id
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);

                    if(user == null)
                    {
                        return new NotFoundViewResult("OwnerNotFound");
                    }

                    user.PhoneNumber = model.MobileNumber;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;

                    // Update the user so that it has an entity related to it
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        model.User = user;

                        var owner = _converterHelper.ToOwner(model, false, imageId);

                        await _ownerRepository.UpdateAsync(owner);
                    }
                    // TODO: Owner could not be updated
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ownerRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("OwnerNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("OwnerNotFound");
            }

            var owner = await _ownerRepository.GetWithUserByIdAsync(id.Value);

            try
            {
                var user = await _userHelper.GetUserByIdAsync(owner.User.Id);

                await _ownerRepository.DeleteAsync(owner);
                await _userHelper.DeleteUserAsync(user);

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
    }
}
