using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Vereyon.Web;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Specialities;

namespace VetManage.Web.Controllers
{
    public class SpecialitiesController : Controller
    {
        private readonly ISpecialitiesRepository _specialitiesRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public SpecialitiesController(
            ISpecialitiesRepository specialitiesRepository,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage)
        {
            _specialitiesRepository = specialitiesRepository;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Index()
        {
            var specialities = _specialitiesRepository.GetAll();

            var model = _converterHelper.AllToSpecialityViewModel(specialities);

            return View(model);
        }

        public IActionResult IndexPublic()
        {
            var specialities = _specialitiesRepository.GetAll();

            var model = _converterHelper.AllToSpecialityViewModel(specialities);

            return View(model);
        }

        public async Task<IActionResult> SingleSpeciality(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var speciality = await _specialitiesRepository.GetByIdAsync(id.Value);

            if(speciality == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var model = _converterHelper.ToSpecialityViewModel(speciality);

            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create(SpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {
                var speciality = _converterHelper.ToSpeciality(model, true);

                _flashMessage.Confirmation("Speciality was created successfully.");

                await _specialitiesRepository.CreateAsync(speciality);

                return View(model);
            }
            _flashMessage.Danger("Could not create speciality.");
            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var speciality = await _specialitiesRepository.GetByIdAsync(id.Value);

            if(speciality == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var model = _converterHelper.ToSpecialityViewModel(speciality);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(SpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var speciality = _converterHelper.ToSpeciality(model, false);

                    await _specialitiesRepository.UpdateAsync(speciality);

                    _flashMessage.Confirmation("Speciality was updated succesfully.");

                    return View(model);
                }
                catch (Exception ex)
                {
                    if(!await _specialitiesRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("SpecialityNotFound");
                    }

                    _flashMessage.Danger(ex.Message);
                }
            }
            _flashMessage.Danger("Could not update speciality succesfully.");

            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var speciality = await _specialitiesRepository.GetByIdAsync(id.Value);

            if(speciality == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            try
            {
                await _specialitiesRepository.DeleteAsync(speciality);

                _flashMessage.Confirmation("Speciality was deleted succesfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // TODO: Vet could not be deleted
                if (!await _specialitiesRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("SpecialityNotFound");
                }

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"You can't delete {speciality.Name}. Too much depends on it";
                    ViewBag.ErrorMessage = $"You can't delete this spciality because there are appointments associated with it.</br></br>" +
                        $"Delete all appointments associated with this speciality and try again.</br></br>" +
                        $"Note: If there are messages associated with this user you may not delete it.";
                }

                return View("Error");
            }
        }

        public IActionResult SpecialityNotFound()
        {
            return View();
        }
    }
}
