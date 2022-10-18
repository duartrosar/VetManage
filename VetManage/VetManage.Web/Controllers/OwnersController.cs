using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models;

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

            OwnersViewModel ownersViewModel = new 
                OwnersViewModel()
            {
                UsersCombo = users,
                Owners = _converterHelper.AllToOwnerViewModel(owners),
                Owner = new OwnerViewModel
                {
                    Users = users,
                },
            };

            return View(ownersViewModel);
        }

        [Route("Owners/IndexPartial")]
        public IActionResult IndexPartial()
        {
            var owners = _ownerRepository.GetAllWithUsers();
            var users = _ownerRepository.GetComboUsers();

            var ownerViewModels = _converterHelper.AllToOwnerViewModel(owners);

            OwnerViewModel ownerViewModel = new OwnerViewModel
            {
                Users = users,
            };

            OwnersViewModel ownersViewModel = new OwnersViewModel()
            {
                UsersCombo = users,
                Owners = ownerViewModels,
                Owner = ownerViewModel
            };

            return PartialView("_IndexPartial",ownersViewModel);
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = $"~/images/noimage.png";

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

                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if(user != null)
                {
                    var owner = _converterHelper.ToOwner(model, true, path);

                    user.HasEntity = true;
                    user.EntityId = owner.Id;

                    // Update the user so that it has a entity related to it
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        owner.User = user;

                        await _ownerRepository.CreateAsync(owner);
                        return RedirectToAction(nameof(Index));
                    }
                    // TODO: Owner could not be created
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
                    user.HasEntity = true;
                    user.EntityId = model.Id;

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _ownerRepository.GetWithUserByIdAsync(id);

            var user = await _userHelper.GetUserByIdAsync(owner.User.Id);
            user.HasEntity = false;
            user.EntityId = -1;

            // Update the user so that it has an entity related to it
            var response = await _userHelper.UpdateUserAsync(user);

            if (response.Succeeded)
            {
                await _ownerRepository.DeleteAsync(owner);
                return RedirectToAction(nameof(Index));
            }
            // TODO: Owner could not be deleted

            return RedirectToAction(nameof(Index));
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
