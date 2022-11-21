using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
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
        private readonly IFlashMessage _flashMessage;

        public PetsController(
            DataContext context,
            IPetRepository petRepository,
            IOwnerRepository ownerRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IFlashMessage flashMessage)
        {
            _context = context;
            _petRepository = petRepository;
            _ownerRepository = ownerRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
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
            var model = new PetViewModel()
            {
                DateOfBirth = DateTime.Now,
            };

            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetViewModel model)
        {
            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);

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

                        model.ImageId = imageId;

                        _flashMessage.Confirmation("Pet was created successfully");

                        return View(model);
                    }
                    _flashMessage.Danger("Owner could not be found, please try again.");
                }
                catch (Exception ex)
                {
                    _flashMessage.Warning(ex.Message);
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

            model.ImageIdString = pet.ImageId.ToString();

            return View(model);
        }

        // POST: Pets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetViewModel model)
        {
            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "pets");
                    }

                    var owner = await _ownerRepository.GetByIdAsync(model.OwnerId);

                    if(owner != null)
                    {
                        var pet = _converterHelper.ToPet(model, false, imageId);

                        await _petRepository.UpdateAsync(pet);

                        model.ImageId = imageId;

                        _flashMessage.Confirmation("Pet was updated successfully");

                        return View(model);
                    }
                    _flashMessage.Danger("Owner could not be found, please try again.");
                }
                catch (Exception ex)
                {
                    if (!await _petRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("PetNotFound");
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
                return new NotFoundViewResult("PetNotFound");

            }

            var pet = await _petRepository.GetByIdAsync(id.Value);

            if (pet == null)
            {
                return new NotFoundViewResult("PetNotFound");
            }

            try
            {
                await _petRepository.DeleteAsync(pet);

                _flashMessage.Confirmation("Pet was deleted successfully");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!await _petRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("PetNotFound");
                }

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"You can't delete {pet.Name}. Too much depends on it";
                    ViewBag.ErrorMessage = $"You can't delete this vet because there are appointments associated with it.</br></br>" +
                        $"Delete all appointments associated with this pet and try again.</br></br>";
                }

                return View("Error");
            }
        }

        public IActionResult PetNotFound()
        {
            return View();
        }
    }
}
