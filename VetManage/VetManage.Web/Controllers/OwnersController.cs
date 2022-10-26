using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class OwnersController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public OwnersController(
            IOwnerRepository ownerRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper)
        {
            _ownerRepository = ownerRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        // GET: Owners
        //[Route("Owners/Index")]
        public IActionResult Index()
        {
            var owners = _ownerRepository.GetAllWithUsers();
            var users = _ownerRepository.GetComboUsers();

            RegisterOwnerViewModel registerOwnerViewModel = new RegisterOwnerViewModel()
            {
                OwnerViewModel = new OwnerViewModel(),
            };

            OwnersManagingViewModel ownersViewModel = new 
                OwnersManagingViewModel()
            {
                Owners = _converterHelper.AllToOwnerViewModel(owners),
                OwnerViewModel = new OwnerViewModel(),
                RegisterOwnerViewModel = registerOwnerViewModel,
            };

            return View(ownersViewModel);
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterOwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = $"~/images/noimage.png";

                if (model.OwnerViewModel.ImageFile != null && model.OwnerViewModel.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\owners",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.OwnerViewModel.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/owners/{file}";
                }

                try
                {
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

                        var result = await _userHelper.AddUserAsync(user, model.Password);

                        if (result != IdentityResult.Success)
                        {
                            ModelState.AddModelError(string.Empty, "The User could not be created.");

                            return View(model);
                        }

                        // Add role or roles to user
                        await _userHelper.AddUserToRoleAsync(user, "Employee");

                        // get the newly created user and set it as the vet's user
                        model.OwnerViewModel.User = await _userHelper.GetUserByEmailAsync(model.Username);

                        var owner = _converterHelper.ToOwner(model.OwnerViewModel, true, path);

                        await _ownerRepository.CreateAsync(owner);

                        return RedirectToAction(nameof(Index));
                        // TODO: Vet could not be created
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return RedirectToAction(nameof(Index));
        }


        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OwnerViewModel model)
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
                        "wwwroot\\images\\owners",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/owners/{file}";
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

                        var owner = _converterHelper.ToOwner(model, false, path);

                        await _ownerRepository.UpdateAsync(owner);
                    }
                    // TODO: Owner could not be updated
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ownerRepository.ExistsAsync(model.Id))
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
            var owner = await _ownerRepository.GetWithUserByIdAsync(id);
            var user = await _userHelper.GetUserByIdAsync(owner.User.Id);

            try
            {
                await _ownerRepository.DeleteAsync(owner);
                await _userHelper.DeleteUserAsync(user);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                // TODO: Vet could not be deleted
                if (!await _ownerRepository.ExistsAsync(id))
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
        [Route("Owners/GetUserAsync")]
        public async Task<JsonResult> GetUserAsync(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            return Json(user);
        }
    }
}
