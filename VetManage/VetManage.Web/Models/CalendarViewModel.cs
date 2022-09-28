using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class CalendarViewModel
    {
        public ICollection<AppointmentViewModel> Appointments { get; set; }

        //public ICollection<Pet> Pets { get; set; }

        //public ICollection<Vet> Vets { get; set; }

        public AppointmentViewModel Appointment { get; set; }
    }
}
