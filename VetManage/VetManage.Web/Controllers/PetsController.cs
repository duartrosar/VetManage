using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetManage.Web.Data;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Owners;
using VetManage.Web.Models.Pets;

namespace VetManage.Web.Controllers
{
    [Authorize]
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
            return View(_petRepository
                .GetAll()
                .OrderBy(p => p.Name));
        }


        // GET: Vets/Create
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // vet not found
                return NotFound();
            }

            var pet = await _petRepository.GetWithOwnerByIdAsync(id.Value);

            if (pet == null)
            {
                // pet not found
                return NotFound();
            }

            var model = new PetDetailsViewModel
            {
                PetViewModel = _converterHelper.ToPetViewModel(pet),
                Owner = pet.Owner
            };



            return View(model);
        }

        public IActionResult Create()
        {
            var pets = _petRepository.GetAllWithOwners();
            var owners = _ownerRepository.GetAllWithUsers();

            //var petViewModels = (ICollection<PetViewModel>)_converterHelper.AllToPetViewModel(pets);
            //var ownerViewModels = (ICollection<OwnerViewModel>)_converterHelper.AllToOwnerViewModel(owners);

            PetsManagingViewModel petsViewModel = new PetsManagingViewModel
            {
                Pets = (ICollection<PetViewModel>)_converterHelper.AllToPetViewModel(pets),
                Owners = (ICollection<OwnerViewModel>)_converterHelper.AllToOwnerViewModel(owners),
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
                var path = string.Empty;

                if(model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\pets",
                        file);

                    using(var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/pets/{file}";
                }

                var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                if(owner != null)
                {
                    var pet = _converterHelper.ToPet(model, true, path);

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
                var path = model.ImageUrl;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    var file = $"{guid}.jpg";

                    path = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot\\images\\pets",
                        file);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/images/pets/{file}";
                }

                try
                {
                    var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                    if(owner != null)
                    {
                        // TODO: replace by image path
                        var pet = _converterHelper.ToPet(model, false, path);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);

            await _petRepository.DeleteAsync(pet);
            return RedirectToAction(nameof(Index));
        }
    }
}
