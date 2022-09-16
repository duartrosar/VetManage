using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetManage.Web.Data;
using VetManage.Web.Data.Entities;
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
        public IActionResult Index()
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
                Users = users,
                Owners = ownerViewModels,
                Owner = ownerViewModel
            };

            return View(ownersViewModel);
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _ownerRepository.GetByIdAsync(id.Value);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OwnerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if(user != null)
                {
                    var owner = _converterHelper.ToOwner(model, true);

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
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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

                        var owner = _converterHelper.ToOwner(model, false);

                        await _ownerRepository.UpdateAsync(model);
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
            //var owner = await _ownerRepository.GetByIdAsync(id);
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
