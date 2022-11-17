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
        private readonly IBlobHelper _blobHelper;

        public PetsController(
            DataContext context,
            IPetRepository petRepository,
            IOwnerRepository ownerRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper)
        {
            _context = context;
            _petRepository = petRepository;
            _ownerRepository = ownerRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
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
                // pet not found
                return new NotFoundViewResult("PetNotFound");
            }

            var pet = await _petRepository.GetWithOwnerByIdAsync(id.Value);

            if (pet == null)
            {
                // pet not found
                return new NotFoundViewResult("PetNotFound");
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
                try
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "pets");
                    }

                    var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                    if (owner != null)
                    {
                        var pet = _converterHelper.ToPet(model, true, imageId);

                        await _petRepository.CreateAsync(pet);
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {
                    // TODO: Pet could not be created

                    throw;
                }
            }
            return View(model);
        }

        // GET: Vets/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                // vet not found
                return new NotFoundViewResult("PetNotFound");

            }

            var pet = await _petRepository.GetWithOwnerByIdAsync(id.Value);

            if (pet == null)
            {
                // pet not found
                return new NotFoundViewResult("PetNotFound");
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
                try
                {
                    Guid imageId = Guid.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "pets");
                    }

                    var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                    if(owner != null)
                    {
                        // TODO: replace by image path
                        var pet = _converterHelper.ToPet(model, false, imageId);
                        await _petRepository.UpdateAsync(pet);
                    }
                    // TODO: Pet could not be updated
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _petRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("PetNotFound");
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
                return new NotFoundViewResult("PetNotFound");

            }


            try
            {
                var pet = await _petRepository.GetByIdAsync(id.Value);

                await _petRepository.DeleteAsync(pet);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                if (!await _petRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("PetNotFound");
                }
                else
                {
                    throw;
                }
                throw;
            }
        }

        public IActionResult PetNotFound()
        {
            return View();
        }
    }
}
