using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models;

namespace VetManage.Web.Controllers
{
    public class VetsController : Controller
    {
        private readonly IVetRepository _vetRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public VetsController(
            IVetRepository vetRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper)
        {
            _vetRepository = vetRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }
        public IActionResult Index()
        {
            var vets = _vetRepository.GetAllWithUsers();
            //var users = _vetRepository.GetComboUsersNoEntity();
            var users = _vetRepository.GetComboUsers();

            var vetViewModels = _converterHelper.AllToVetViewModel(vets);

            VetViewModel vetViewModel = new VetViewModel
            {
                Users = users,
            };

            VetsViewModel vetsViewModel = new VetsViewModel()
            {
                //Users = users,
                Vets = vetViewModels,
                Vet = vetViewModel,
            };

            return View(vetsViewModel);
        }

        // POST: Vets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VetViewModel model)
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
                        "wwwroot\\images\\vets",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/vets/{file}";
                }

                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if (user != null)
                {
                    var owner = _converterHelper.ToVet(model, true, path);

                    user.HasEntity = true;
                    user.EntityId = owner.Id;

                    // Update the user so that it has an entity related to it
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        owner.User = user;

                        await _vetRepository.CreateAsync(owner);
                        return RedirectToAction(nameof(Index));
                    }
                    // TODO: Vet could not be created
                }
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
                    user.HasEntity = true;
                    user.EntityId = model.Id;

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vet = await _vetRepository.GetWithUserByIdAsync(id);

            var user = await _userHelper.GetUserByIdAsync(vet.User.Id);
            user.HasEntity = false;
            user.EntityId = -1;

            // Update the user so that it has an entity related to it
            var response = await _userHelper.UpdateUserAsync(user);

            if (response.Succeeded)
            {
                await _vetRepository.DeleteAsync(vet);
                return RedirectToAction(nameof(Index));
            }
            // TODO: Vet could not be deleted

            return RedirectToAction(nameof(Index));
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
