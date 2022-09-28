using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VetManage.Web.Data;
using VetManage.Web.Data.Entities;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models;

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

        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
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
            return RedirectToAction(nameof(Index));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            await _appointmentRepository.DeleteAsync(appointment);

            return RedirectToAction(nameof(Index));
        }
    }
}
