using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VetManage.Web.Data.Repositories;
using VetManage.Web.Helpers;
using VetManage.Web.Models;
using VetManage.Web.Models.Account;
using VetManage.Web.Models.Home;

namespace VetManage.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMessageBoxRepository _messageBoxRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IPetRepository _petRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public HomeController(
            ILogger<HomeController> logger, 
            IConverterHelper converterHelper, 
            ITreatmentRepository treatmentRepository, 
            IPetRepository petRepository, 
            IAppointmentRepository appointmentRepository,
            IOwnerRepository ownerRepository,
            IMessageBoxRepository messageBoxRepository,
            IUserHelper userHelper)
        {
            _logger = logger;
            _converterHelper = converterHelper;
            _treatmentRepository = treatmentRepository;
            _petRepository = petRepository;
            _appointmentRepository = appointmentRepository;
            _ownerRepository = ownerRepository;
            _messageBoxRepository = messageBoxRepository;
            _userHelper = userHelper;
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var todaysAppointments = _appointmentRepository.GetAllFromToday();
            var latestAppointments = _appointmentRepository.GetMostRecentlyBooked();
            var latestTreatments = _treatmentRepository.GetLatestTreatments();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userHelper.GetUserByIdAsync(userId);

            if(user != null)
            {
                // TODO: If user is owner
                ViewData["User"] = user.FullName;
            }

            var unreadMessages = await _messageBoxRepository.GetUnreadMessages(userId);

            int totalPets = _petRepository.GetAll().Count();
            int totalOwners = _ownerRepository.GetAll().Count();
            int totalAppointments = _appointmentRepository.GetAll().Count();


            var owner = await _ownerRepository.GetByUserIdWithPetsAsync(userId);

            if(owner != null)
            {
                todaysAppointments = _appointmentRepository.GetAllByOwnerIdFromToday(owner.Id);
                latestAppointments = _appointmentRepository.GetMostRecentlyBookedByOwnerId(owner.Id);
                latestTreatments = _treatmentRepository.GetLatestTreatmentsByOwnerId(owner.Id);
            }

            DashBoardViewModel model = new DashBoardViewModel()
            {
                TodaysAppoinments = _converterHelper.AllToAppointmentViewModel(todaysAppointments),
                LatestAppointments = _converterHelper.AllToAppointmentViewModel(latestAppointments),
                LastestTreatments = _converterHelper.AllToTreatmentViewModel(latestTreatments),
                UnreadMessages = _converterHelper.AllToMessageViewModel(unreadMessages),
                TotalPets = totalPets,
                TotalOwners = totalOwners,
                TotalAppointments = totalAppointments,
            };

            return View(model);
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Dashboard));
            }
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
