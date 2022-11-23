using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Vets;

namespace VetManage.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VetsController : Controller
    {
        private readonly IVetRepository _vetRepository;
        private readonly IMessageBoxRepository _messageBoxRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IMessageHelper _messageHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IFlashMessage _flashMessage;

        public VetsController(
            IVetRepository vetRepository,
            IMessageBoxRepository messageBoxRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IMessageHelper messageHelper,
            IBlobHelper blobHelper,
            IFlashMessage flashMessage)
        {
            _vetRepository = vetRepository;
            _messageBoxRepository = messageBoxRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _messageHelper = messageHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
        }
        public IActionResult Index()
        {
            return View(_vetRepository.GetAll()
                .OrderBy(v => v.FirstName)
                .ThenBy(v => v.LastName));
        }


        // GET: Vets/Create
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("VetNotFound");
            }

            var vet = await _vetRepository.GetWithUserByIdAsync(id.Value);

            if (vet == null)
            {
                // vet not found
                return new NotFoundViewResult("VetNotFound");
            }

            var model = new VetDetailsViewModel
            {
                VetViewModel = _converterHelper.ToVetViewModel(vet),
                Username = vet.User.UserName,
            };

            return View(model);
        }

        // GET: Vets/Create
        public IActionResult Create()
        {
            var model = new RegisterVetViewModel
            {
                VetViewModel = new VetViewModel
                {
                    DateOfBirth = DateTime.Now,
                },
            };

            ViewData["Genders"] = _converterHelper.GetGenders();

            return View(model);
        }

        // POST: Vets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterVetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.VetViewModel.ImageId;

                    if (model.VetViewModel.ImageFile != null && model.VetViewModel.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.VetViewModel.ImageFile, "vets");
                    }

                    var user = await _userHelper.GetUserByEmailAsync(model.Username);

                    if (user == null)
                    {
                        // Convert Vet
                        var vet = _converterHelper.ToVet(model.VetViewModel, true, imageId);

                        user = _converterHelper.ToUser(vet, new User(), "vets");

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
                        await _userHelper.AddUserToRoleAsync(user, "Employee");

                        if (model.IsAdmin)
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Admin");
                        }

                        // get the newly created user and set it as the vet's user
                        vet.User = await _userHelper.GetUserByEmailAsync(model.Username);

                        // Save Vet
                        await _vetRepository.CreateAsync(vet);

                        // Create user's MessageBox
                        await _messageHelper.InitializeMessageBox(user.Id);

                        Response response = await ConfirmEmailAsync(user, model);

                        if (response.IsSuccess)
                        {
                            _flashMessage.Confirmation("Vet has been created and confirmation email has been sent to user.");
                        }

                        model.VetViewModel.ImageId = imageId;

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

        // GET: Vets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                // vet not found
                return new NotFoundViewResult("VetNotFound");
            }

            var vet = await _vetRepository.GetWithUserByIdAsync(id.Value);

            if(vet == null)
            {
                // vet not found
                return new NotFoundViewResult("VetNotFound");
            }

            var model = _converterHelper.ToVetViewModel(vet);

            if(await _userHelper.IsUserInRoleAsync(vet.User, "Admin"))
            {
                model.IsAdmin = true;
            }

            ViewData["Genders"] = _converterHelper.GetGenders();

            return View(model);
        }

        // POST: Vets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VetViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewData["Genders"] = _converterHelper.GetGenders();

                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "vets");
                    }

                    // Get the user related with the vet
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);

                    if(user == null)
                    {
                        return new NotFoundViewResult("VetNotFound");
                    }

                    var vet = _converterHelper.ToVet(model, false, imageId);

                    user = _converterHelper.ToUser(vet, user, "vets");

                    model.ImageId = imageId;

                    // Update the user 
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        // Change user's role
                        if (model.IsAdmin)
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Admin");
                        }
                        else
                        {
                            await _userHelper.RemoveUserFromRoleAsync(user, "Admin");
                        }

                        model.User = user;

                        vet.UserId = user.Id;

                        await _vetRepository.UpdateAsync(vet);

                        _flashMessage.Confirmation("Vet has been updated.");

                        return View(model);
                    }
                    _flashMessage.Danger("An error ocurred whilst tryng to update the vet, please try again.");

                    return View(model);
                }
                catch (Exception ex)
                {
                    if (!await _vetRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("VetNotFound");
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
                // vet not found
                return new NotFoundViewResult("VetNotFound");
            }

            var vet = await _vetRepository.GetWithUserByIdAsync(id.Value);

            if (vet == null || vet.User == null)
            {
                // vet not found
                return new NotFoundViewResult("VetNotFound");
            }

            try
            {

                var user = await _userHelper.GetUserByIdAsync(vet.User.Id);

                await _userHelper.DeleteUserAsync(user);

                _flashMessage.Confirmation("Vet deleted successfully");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // TODO: Vet could not be deleted
                if (!await _vetRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("VetNotFound");
                }

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"You can't delete {vet.FullName}. Too much depends on it";
                    ViewBag.ErrorMessage = $"You can't delete this vet because there are messages and appointments associated with it.</br></br>" +
                        $"Delete all appointments associated with this user and try again.</br></br>" +
                        $"Note: If there are messages associated with this user you may not delete it.";
                }

                return View("Error");
            }
        }

        [HttpPost]
        [Route("Vets/GetUserAsync")]
        public async Task<JsonResult> GetUserAsync(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            return Json(user);
        }

        public IActionResult VetNotFound()
        {
            return View();
        }

        public async Task<Response> ConfirmEmailAsync(User user, RegisterVetViewModel model)
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
