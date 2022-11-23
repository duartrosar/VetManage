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
        private readonly ISpecialityRepository _specialityRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public SpecialitiesController(
            ISpecialityRepository specialityRepository,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage)
        {
            _specialityRepository = specialityRepository;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Index()
        {
            var specialities = _specialityRepository.GetAll();

            var model = _converterHelper.AllToSpecialityViewModel(specialities);

            return View(model);
        }

        public IActionResult IndexPublic()
        {
            var specialities = _specialityRepository.GetAll();

            var model = _converterHelper.AllToSpecialityViewModel(specialities);

            return View(model);
        }

        public async Task<IActionResult> SingleSpeciality(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var speciality = await _specialityRepository.GetByIdAsync(id.Value);

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

                await _specialityRepository.CreateAsync(speciality);

                return RedirectToAction(nameof(Index));
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

            var speciality = await _specialityRepository.GetByIdAsync(id.Value);

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

                    await _specialityRepository.UpdateAsync(speciality);

                    _flashMessage.Confirmation("Speciality was updated succesfully.");

                    return View(model);
                }
                catch (Exception ex)
                {
                    if(!await _specialityRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("SpecialityNotFound");
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
                return new NotFoundViewResult("SpecialityNotFound");
            }

            var speciality = await _specialityRepository.GetByIdAsync(id.Value);

            if(speciality == null)
            {
                return new NotFoundViewResult("SpecialityNotFound");
            }

            try
            {
                await _specialityRepository.DeleteAsync(speciality);

                _flashMessage.Confirmation("Speciality was deleted succesfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!await _specialityRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("SpecialityNotFound");
                }

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"You can't delete {speciality.Name}. Too much depends on it";
                    ViewBag.ErrorMessage = $"You can't delete this speciality because there are treatments associated with it.</br></br>" +
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
