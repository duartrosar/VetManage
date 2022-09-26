using System.Collections.Generic;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class CalendarViewModel
    {
        public ICollection<Appointment> Appointments { get; set; }

        public Appointment Appointment { get; set; }
    }
}
