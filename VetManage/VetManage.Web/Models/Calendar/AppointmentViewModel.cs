using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Calendar
{
    public class AppointmentViewModel : Appointment
    {

        public IEnumerable<SelectListItem> ComboVets { get; set; }

        public IEnumerable<SelectListItem> ComboPets { get; set; }

        [Required]
        public string StartTimeString { get; set; }

        [Required]
        public string EndTimeString { get; set; }

        // Gets the StartTime date in a format to be used by the date input in the frontend
        public string StartDateShort
        {
            get 
            { 
                return StartTime.ToString("yyyy-MM-dd"); 
            }
        }

        // Gets the StartTime date in a format to be used by the timepickers in the frontend
        public string StartDateLong
        {
            get
            {
                return StartTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }
        }

        // Gets the EndTime date in a format to be used by the timepickers in the frontend
        public string EndDateLong
        {
            get
            {
                return EndTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }
        }
        //[Display(Name = "Pet")]
        //[Range(1, int.MaxValue, ErrorMessage = "You must select a Pet")]
        //public int PetId { get; set; }

        //[Display(Name = "Vet")]
        //[Range(1, int.MaxValue, ErrorMessage = "You must select a Vet")]
        //public int VetId { get; set; }
    }
}
