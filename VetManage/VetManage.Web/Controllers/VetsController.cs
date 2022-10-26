using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Vets;

namespace VetManage.Web.Controllers
{
    public class VetsController : Controller
    {
        private readonly IVetRepository _vetRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public VetsController(
            IVetRepository vetRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            IMailHelper mailHelper)
        {
            _vetRepository = vetRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }
        public IActionResult Index()
        {
            var vets = _vetRepository.GetAllWithUsers();
            //var users = _vetRepository.GetComboUsersNoEntity();
            var users = _vetRepository.GetComboUsers();

            var vetViewModels = _converterHelper.AllToVetViewModel(vets);

            RegisterVetViewModel registerVetViewModel = new RegisterVetViewModel
            {
                VetViewModel = new VetViewModel(),
            };

            VetsManagingViewModel vetsViewModel = new VetsManagingViewModel()
            {
                //Users = users,
                Vets = vetViewModels,
                VetViewModel = new VetViewModel(),
                RegisterVetViewModel = registerVetViewModel,
            };

            return View(vetsViewModel);
        }

        // POST: Vets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterVetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = $"~/images/noimage.png";

                if (model.VetViewModel.ImageFile != null && model.VetViewModel.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\vets",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.VetViewModel.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/vets/{file}";
                }

                try
                {
                    var user = await _userHelper.GetUserByEmailAsync(model.Username);

                    if (user == null)
                    {
                        // Create new user
                        user = new User
                        {
                            FirstName = model.VetViewModel.FirstName,
                            LastName = model.VetViewModel.LastName,
                            Email = model.Username,
                            UserName = model.Username,
                            Address = model.VetViewModel.Address,
                            PhoneNumber = model.VetViewModel.MobileNumber,
                            PasswordChanged = false,
                        };

                        var result = await _userHelper.AddUserAsync(user, model.Password);

                        if (result != IdentityResult.Success)
                        {
                            ModelState.AddModelError(string.Empty, "The User could not be created.");

                            return View(model);
                        }

                        // Add role or roles to user
                        await _userHelper.AddUserToRoleAsync(user, "Employee");

                        if (model.IsAdmin)
                        {
                            await _userHelper.AddUserToRoleAsync(user, "Admin");
                        }

                        // get the newly created user and set it as the vet's user
                        model.VetViewModel.User = await _userHelper.GetUserByEmailAsync(model.Username);

                        var vet = _converterHelper.ToVet(model.VetViewModel, true, path);

                        await _vetRepository.CreateAsync(vet);

                        // Send confirmation and change password email
                        string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        string tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userId = user.Id,
                            token = myToken,
                        }, protocol: HttpContext.Request.Scheme);

                        Response response = _mailHelper.SendEmail(
                            model.Username,
                            "Email Confirmation",
                            $"<h1>Email Confirmation</h1>" +
                            $"To allow the user, " +
                            $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                        if (response.IsSuccess)
                        {
                            ViewBag.Message = "Confirmation email has been sent";
                            return RedirectToAction(nameof(Index));
                        }

                        return RedirectToAction(nameof(Index));
                        // TODO: Vet could not be created
                    }
                    else
                    {
                        // TODO: User already exists
                    }
                }
                catch (Exception ex)
                {

                }
                // Check if user exists
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Vets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = model.ImageUrl;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\vets",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/vets/{file}";
                }

                try
                {
                    // Get the user the user chose from the dropdown with the id
                    var user = await _userHelper.GetUserByIdAsync(model.UserId);

                    user.PhoneNumber = model.MobileNumber;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;

                    // Update the user so that it has an entity related to it
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        model.User = user;

                        var vet = _converterHelper.ToVet(model, false, path);

                        await _vetRepository.UpdateAsync(vet);
                    }
                    // TODO: Vet could not be updated
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _vetRepository.ExistsAsync(model.Id))
                    {
                        return NotFound();
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

        // POST: Owners/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var vet = await _vetRepository.GetWithUserByIdAsync(id);
            var user = await _userHelper.GetUserByIdAsync(vet.User.Id);

            try
            {
                await _vetRepository.DeleteAsync(vet);
                await _userHelper.DeleteUserAsync(user);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                // TODO: Vet could not be deleted
                if (!await _vetRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("Vets/GetUserAsync")]
        public async Task<JsonResult> GetUserAsync(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            return Json(user);
        }
    }
}
