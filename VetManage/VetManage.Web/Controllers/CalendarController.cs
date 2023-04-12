using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vereyon.Web;
using VetManage.Web.Data;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Calendar;

namespace VetManage.Web.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IVetRepository _vetRepository;
        private readonly IPetRepository _petRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;

        public CalendarController(
            IAppointmentRepository appointmentRepository,
            IConverterHelper converterHelper,
            IVetRepository vetRepository,
            IPetRepository petRepository,
            IOwnerRepository ownerRepository,
            IFlashMessage flashMessage,
            IUserHelper userHelper)
        {
            _appointmentRepository = appointmentRepository;
            _converterHelper = converterHelper;
            _vetRepository = vetRepository;
            _petRepository = petRepository;
            _ownerRepository = ownerRepository;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = _appointmentRepository.GetAll();

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            if (owner != null)
            {
                appointments = (IQueryable<Appointment>)_appointmentRepository.GetAllByOwnerId(owner.Id);
            }

            var model = _converterHelper.AllToAppointmentViewModel(appointments);

            return View(model);
        }


        public async Task<IActionResult> Create(string date)
        {
            DateTime dateTime; ;

            // If the user clicks the new appointment button
            if (date == null)
            {
                // Make sure slected time is 7 am
                TimeSpan time = new TimeSpan(7, 0, 0);
                dateTime = DateTime.Now;
                dateTime = dateTime.Date + time;
            } else
            {
                dateTime = DateTime.Parse(date);
            }

            var pets = _petRepository.GetAllWithOwners();
            var vets = _vetRepository.GetAllWithUsers();

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var owner = await _ownerRepository.GetByUserIdAsync(userId);

            if (owner != null)
            {
                pets = _petRepository.GetAllByOwnerIdAsync(owner.Id);
            }

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);
            ViewData["Vets"] = _converterHelper.AllToVetViewModel(vets);
            ViewData["DateString"] = dateTime.ToString("yyyy-MM-dd");
            ViewData["Date"] = dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var pet = await _petRepository.GetByIdAsync(model.PetId);
                    var vet = await _vetRepository.GetByIdAsync(model.VetId);

                    // if both the pet and vet exist
                    var isNull = pet != null && vet != null;

                    if (isNull)
                    {
                        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        var user = await _userHelper.GetUserByIdAsync(userId);

                        if(user != null)
                        {
                            var appointment = _converterHelper.ToAppointment(model, true);

                            appointment.User = user;

                            await _appointmentRepository.CreateAsync(appointment);

                            _flashMessage.Confirmation("Appointment booked successfully.");

                            return RedirectToAction(nameof(Index));
                        }

                    }

                    _flashMessage.Danger("Appointment could not be booked.");
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger(ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var appointment = await _appointmentRepository.GetWithMembersByIdAsync(id.Value);

            if(appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var model = _converterHelper.ToAppointmentViewModel(appointment);

            var pets = _petRepository.GetAllWithOwners();
            var vets = _vetRepository.GetAllWithUsers();

            // Get the logged in user to check if it's an owner
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var owner = await _ownerRepository.GetByUserIdWithPetsAsync(userId);

            if (owner != null)
            {
                // the user is an owner and is trying to access an appointment that doesn't belong to one of their pets
                if(appointment.Pet.OwnerId != owner.Id)
                {
                    return new NotFoundViewResult("AppointmentNotFound");
                }

                pets = _petRepository.GetAllByOwnerIdAsync(owner.Id);
            }

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);
            ViewData["Vets"] = _converterHelper.AllToVetViewModel(vets);
            ViewData["DateString"] = model.StartTime.ToString("yyyy-MM-dd");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentViewModel model)
        {
            var pets = _petRepository.GetAllWithOwners();
            var vets = _vetRepository.GetAllWithUsers();

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);
            ViewData["Vets"] = _converterHelper.AllToVetViewModel(vets);
            ViewData["DateString"] = model.StartTime.ToString("yyyy-MM-dd");

            var pet = await _petRepository.GetByIdAsync(model.PetId);
            var vet = await _vetRepository.GetByIdAsync(model.VetId);

            // if both the pet and vet exist
            var notNull = pet != null && vet != null;

            if (ModelState.IsValid)
            {
                try
                {
                    if (notNull)
                    {
                        var appointment = _converterHelper.ToAppointment(model, false);

                        await _appointmentRepository.UpdateAsync(appointment);

                        _flashMessage.Confirmation("Appointment booked successfully.");

                        model.Pet = pet;
                        model.Vet = vet;

                        return View(model);
                    }

                    return new NotFoundViewResult("AppointmentNotFound");
                }
                catch (Exception ex)
                {
                    if (!await _appointmentRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("AppointmentNotFound");
                    }

                    _flashMessage.Danger(ex.Message);
                }
            }

            if (notNull)
            {
                model.Pet = pet;
                model.Vet = vet;
            }

            return View(model);
        }

        [HttpPost]
        [Route("Calendar/EventDrag")]
        public async Task<JsonResult> EventDrag(AppointmentViewModel model)
        {
            //Console.WriteLine(model);
            var appointment = _converterHelper.ToAppointment(model, false);

            await _appointmentRepository.UpdateAsync(appointment);

            return Json(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var appointment = await _appointmentRepository.GetByIdAsync(id.Value);

            if(appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            try
            {
                await _appointmentRepository.DeleteAsync(appointment);

                _flashMessage.Confirmation("Appointment deleted successfully.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if(!await _appointmentRepository.ExistsAsync(id.Value))
                {
                    return new NotFoundViewResult("AppointmentNotFound");
                }
                _flashMessage.Danger(ex.Message);
            }

            _flashMessage.Danger("Could not delete appointment.");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateRequest()
        {
            return View();
        }

        public IActionResult AppointmentNotFound()
        {
            return View();
        }
    }
}
