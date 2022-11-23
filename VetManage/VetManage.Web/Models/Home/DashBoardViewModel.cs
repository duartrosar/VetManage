using System.Collections.Generic;
using VetManage.Web.Models.Calendar;
using VetManage.Web.Models.Messages;
using VetManage.Web.Models.Treatments;

namespace VetManage.Web.Models.Home
{
    public class DashBoardViewModel
    {
        public IEnumerable<AppointmentViewModel> TodaysAppoinments { get; set; }

        public IEnumerable<AppointmentViewModel> LatestAppointments { get; set; }

        public IEnumerable<TreatmentViewModel> LastestTreatments { get; set; }

        public IEnumerable<MessageViewModel> UnreadMessages { get; set; }

        public int TotalOwners { get; set; }

        public int TotalPets { get; set; }

        public int TotalAppointments { get; set; }
    }
}
