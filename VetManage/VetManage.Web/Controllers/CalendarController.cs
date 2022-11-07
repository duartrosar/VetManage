using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using VetManage.Web.Data;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models.Calendar;

namespace VetManage.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IVetRepository _vetRepository;
        private readonly IPetRepository _petRepository;

        public CalendarController(
            IAppointmentRepository appointmentRepository,
            IConverterHelper converterHelper,
            IVetRepository vetRepository,
            IPetRepository petRepository)
        {
            _appointmentRepository = appointmentRepository;
            _converterHelper = converterHelper;
            _vetRepository = vetRepository;
            _petRepository = petRepository;
        }

        public IActionResult Index()
        {
            var appointments = _appointmentRepository.GetAll();

            CalendarViewModel viewModel = new CalendarViewModel
            {
                Appointments = _converterHelper.AllToAppointmentViewModel(appointments),
                Appointment = new AppointmentViewModel()
                {
                    ComboPets = _appointmentRepository.GetComboPets(),
                    ComboVets = _appointmentRepository.GetComboVets(),
                },
            };

            return View(viewModel);
        }


        public IActionResult Create(string date)
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
                        var appointment = _converterHelper.ToAppointment(model, true);

                        await _appointmentRepository.CreateAsync(appointment);
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetWithMembersByIdAsync(id.Value);

            if(appointment == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToAppointmentViewModel(appointment);

            var pets = _petRepository.GetAllWithOwners();
            var vets = _vetRepository.GetAllWithUsers();

            ViewData["Pets"] = _converterHelper.AllToPetViewModel(pets);
            ViewData["Vets"] = _converterHelper.AllToVetViewModel(vets);
            ViewData["DateString"] = model.StartTime.ToString("yyyy-MM-dd");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentViewModel model)
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
                        var appointment = _converterHelper.ToAppointment(model, false);

                        await _appointmentRepository.UpdateAsync(appointment);

                        return RedirectToAction(nameof(Index));
                    }
                    // TODO: Appointment could not be updated
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _appointmentRepository.ExistsAsync(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));
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
                return NotFound();
            }

            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id.Value);
                await _appointmentRepository.DeleteAsync(appointment);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if(!await _appointmentRepository.ExistsAsync(id.Value))
                {
                    return NotFound();
                } else
                {
                    throw;
                }
            }
        }
    }
}
