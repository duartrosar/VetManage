using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Vereyon.Web;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Treatments;

namespace VetManage.Web.Controllers
{
    [Authorize]
    public class TreatmentsController : Controller
    {
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IPetRepository _petRepository;
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IOwnerRepository _ownerRepository;

        public TreatmentsController(
            ITreatmentRepository treatmentRepository,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage,
            IPetRepository petRepository,
            ISpecialityRepository specialityRepository,
            IOwnerRepository ownerRepository)
        {
            _treatmentRepository = treatmentRepository;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
            _petRepository = petRepository;
            _specialityRepository = specialityRepository;
            _ownerRepository = ownerRepository;
        }
        public async Task<IActionResult> Index()
        {
            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            var treatments = _treatmentRepository.GetAllWithPetsAndSpecialities();

            if (owner != null)
            {
                treatments = _treatmentRepository.GetAllByOwnerId(owner.Id);
            }

            var model = _converterHelper.AllToTreatmentViewModel(treatments);

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            var treatment = await _treatmentRepository.GetWithPetAndSpecialityByIdAsync(id.Value);

            if (treatment == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            if (owner != null)
            {
                if(owner.Id != treatment.Pet.OwnerId)
                {
                    return new NotFoundViewResult("TreatmentNotFound");
                }
            }

            // TODO: Check if the treatment belongs to a pet that belongs 
            // To the logged in user (who's an owner)
            var model = _converterHelper.ToTreatmentViewModel(treatment);

            model.Specialities = _specialityRepository.GetComboSpecialities();

            var pets = _petRepository.GetAllWithOwners();

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);

            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            var pets = _petRepository.GetAllWithOwners();

            var model = new TreatmentViewModel
            {
                Specialities = _specialityRepository.GetComboSpecialities(),
                TreatmentDate = DateTime.Now,
            };

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(TreatmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var treatment = _converterHelper.ToTreatment(model, true);

                    await _treatmentRepository.CreateAsync(treatment);

                    _flashMessage.Confirmation("Treatment was created successfully");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            var treatment = await _treatmentRepository.GetWithPetAndSpecialityByIdAsync(id.Value);

            if(treatment == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            var model = _converterHelper.ToTreatmentViewModel(treatment);

            model.Specialities = _specialityRepository.GetComboSpecialities();

            var pets = _petRepository.GetAllWithOwners();

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(TreatmentViewModel model)
        {
            // Make sure we have the vets and specialities for when return the View
            var pets = _petRepository.GetAllWithOwners();

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);

            model.Specialities = _specialityRepository.GetComboSpecialities();

            var pet = await _petRepository.GetByIdAsync(model.PetId);

            if(pet == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            model.Pet = pet;

            if (ModelState.IsValid)
            {
                try
                {
                    var treatment = _converterHelper.ToTreatment(model, false);

                    await _treatmentRepository.UpdateAsync(treatment);

                    _flashMessage.Confirmation("Treatment updated");

                    return View(model);
                }
                catch (Exception ex)
                {
                    if(!await _treatmentRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("TreatmentNotFound");
                    }

                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            var treatment = await _treatmentRepository.GetByIdAsync(id.Value);

            if(treatment == null)
            {
                return new NotFoundViewResult("TreatmentNotFound");
            }

            try
            {
                await _treatmentRepository.DeleteAsync(treatment);

                _flashMessage.Confirmation("Treatment was deleted successfully");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!await _specialityRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("TreatmentNotFound");
                }

                _flashMessage.Danger("Could not delete treatment.");

                return RedirectToAction(nameof(Index));

                //if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                //{
                //    ViewBag.ErrorTitle = $"You can't delete that treatment. Too much depends on it";
                //    ViewBag.ErrorMessage = $"You can't delete this treatment because there are appointments associated with it.</br></br>" +
                //        $"Delete all appointments associated with this speciality and try again.</br></br>" +
                //        $"Note: If there are messages associated with this user you may not delete it.";
                //}

                //return View("Error");
            }
        }


        public IActionResult TreatmentNotFound()
        {
            return View();
        }
    }
}
