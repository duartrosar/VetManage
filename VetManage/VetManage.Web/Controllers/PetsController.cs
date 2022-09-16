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
    public class PetsController : Controller
    {
        private readonly DataContext _context;
        private readonly IPetRepository _petRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IConverterHelper _converterHelper;

        public PetsController(
            DataContext context,
            IPetRepository petRepository,
            IOwnerRepository ownerRepository,
            IConverterHelper converterHelper)
        {
            _context = context;
            _petRepository = petRepository;
            _ownerRepository = ownerRepository;
            _converterHelper = converterHelper;
        }

        // GET: Pets
        public IActionResult Index()
        {
            var owners = _ownerRepository.GetAllWithPetsAndUsers();
            var pets = _petRepository.GetAllWithOwners();

            var ownerViewModels = _converterHelper.AllToOwnerViewModel(owners);
            var petViewModels = _converterHelper.AllToPetViewModel(pets);

            ViewData["OwnerId"] = new SelectList(ownerViewModels, "Id", "FullName");

            PetsViewModel petsViewModel = new PetsViewModel
            {
                Owners = ownerViewModels,
                Pets = petViewModels,
                Pet = new PetViewModel()
            };

            return View(petsViewModel);
        }


        // POST: Pets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                if(owner != null)
                {
                    var pet = _converterHelper.ToPet(model, true);

                    await _petRepository.CreateAsync(pet);
                    return RedirectToAction(nameof(Index));
                }
                // TODO: Pet could not be created
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                    if(owner != null)
                    {
                        var pet = _converterHelper.ToPet(model, false);
                        await _petRepository.UpdateAsync(pet);
                    }
                    // TODO: Pet could not be updated
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _petRepository.ExistsAsync(model.Id))
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

        // POST: Pets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);

            await _petRepository.DeleteAsync(pet);
            return RedirectToAction(nameof(Index));
        }
    }
}
