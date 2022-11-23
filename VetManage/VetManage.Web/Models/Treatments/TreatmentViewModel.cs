using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Treatments
{
    public class TreatmentViewModel : Treatment
    {
        [Required(ErrorMessage = "You must choose a speciality.")]
        string SpecialityName { get; set; }

        string PetName { get; set; }

        public string DateString { get; set; }

        public IEnumerable<SelectListItem> Specialities { get; set; }

        public string NotesAbbreviation { get; set; }
    }
}
