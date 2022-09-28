using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class AppointmentViewModel : Appointment
    {

        public IEnumerable<SelectListItem> ComboVets { get; set; }

        public IEnumerable<SelectListItem> ComboPets { get; set; }


        [Display(Name = "Pet")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Pet")]
        public int PetId { get; set; }

        [Display(Name = "Vet")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Vet")]
        public int VetId { get; set; }
    }
}
