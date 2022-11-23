using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        private readonly IUserHelper _userHelper;

        public PetsController(
            DataContext context,
            IPetRepository petRepository,
            IOwnerRepository ownerRepository,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IFlashMessage flashMessage,
            IUserHelper userHelper)
        {
            _context = context;
            _petRepository = petRepository;
            _ownerRepository = ownerRepository;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
        }

        //[Authorize(Roles ="Admin,Employee")]
        public async Task<IActionResult> Index()
        {
            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var owner = await _ownerRepository.GetByUserIdAsync(userId);
            
            if(owner != null)
            {
                return View(_petRepository.GetAllByOwnerIdAsync(owner.Id));
            }

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

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            if (owner != null)
            {
                // Check if the user is trying to edit a pet that isn't theirs
                if (owner.Id != pet.OwnerId)
                {
                    return new NotFoundViewResult("PetNotFound");
                }
            }

            var model = _converterHelper.ToPetViewModel(pet);

            return View(model);
        }

        [Authorize(Roles="Employee")]
        public IActionResult Create()
        {
            var model = new PetViewModel()
            {
                DateOfBirth = DateTime.Now,
            };

            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);
            ViewData["Genders"] = _converterHelper.GetPetGenders();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee")]
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

                        return RedirectToAction(nameof(Index));
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

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            if(owner != null)
            {
                // Check if the user is trying to edit a pet that isn't theirs
                if (owner.Id != pet.OwnerId)
                {
                    return new NotFoundViewResult("PetNotFound");
                }
            }

            var owners = _ownerRepository.GetAllWithUsers();

            ViewData["Owners"] = _converterHelper.AllToOwnerViewModel(owners);

            ViewData["Genders"] = _converterHelper.GetPetGenders();

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
            ViewData["Genders"] = _converterHelper.GetPetGenders();

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

                        model.Owner = owner;

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
                    
                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles ="Employee")]
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

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            if(owner != null)
            {
                // Check if the user is trying to edit a pet that isn't theirs
                if (owner.Id != pet.OwnerId)
                {
                    return new NotFoundViewResult("PetNotFound");
                }
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
