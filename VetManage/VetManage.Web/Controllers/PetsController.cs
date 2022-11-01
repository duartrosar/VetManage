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
            return View(_petRepository.GetAllWithOwners());
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

            var model = _converterHelper.ToPetViewModel(pet);

            return View(model);
        }

        public IActionResult Create()
        {
            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);

            return View();
        }

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

            return View(model);
        }

        // GET: Vets/Edit
        public async Task<IActionResult> Edit(int? id)
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

            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);

            var model = _converterHelper.ToPetViewModel(pet);

            return View(model);
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


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                // vet not found
                return NotFound();
            }

            var pet = await _petRepository.GetByIdAsync(id.Value);

            try
            {
                await _petRepository.DeleteAsync(pet);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                if (!await _petRepository.ExistsAsync(id.Value))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                throw;
            }
        }
    }
}
