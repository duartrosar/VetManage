using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Treatment : IEntity
    {
        public int Id { get; set; }

        [Required]
        public DateTime TreatmentDate { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Speciality")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Speciality")]
        public int SpecialityId { get; set; }

        public Speciality Speciality { get; set; }


        [Display(Name = "Pet")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Pet")]
        public int PetId { get; set; }

        public Pet Pet { get; set; }
    }
}
