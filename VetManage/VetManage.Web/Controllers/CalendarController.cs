using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using VetManage.Web.Data;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models;

namespace VetManage.Web.Controllers
{
    public class CalendarController : Controller
    {
        private readonly DataContext _dataContext;

        public CalendarController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            ICollection<Appointment> appointments = new List<Appointment>();

            appointments.Add(new Appointment
            {
                Id = 1,
                Subject = "Appointment 1",
                StartTime = new DateTime(2022, 9, 23, 13, 0, 0),
                EndTime = new DateTime(2022, 9, 23, 14, 0, 0),
            });

            appointments.Add(new Appointment
            {
                Id = 2,
                Subject = "Appointment 2",
                StartTime = new DateTime(2022, 9, 23, 13, 0, 0),
                EndTime = new DateTime(2022, 9, 23, 15, 0, 0),
            });

            CalendarViewModel viewModel = new CalendarViewModel
            {
                Appointments = appointments,
                Appointment = new Appointment(),
            };

            return View(viewModel);
        }

        public IActionResult Create(CalendarViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return RedirectToAction(nameof(Index));
        }
    }
}
