using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Treatment : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must pick a treatment date")]
        public DateTime TreatmentDate { get; set; }

        [Required(ErrorMessage = "You must enter some notes")]
        [MaxLength(500)]
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
