using System;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Helpers;

namespace VetManage.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "You must enter a subject")]
        [MaxLength(75)]
        public string Title { get; set; }

        [Required(ErrorMessage = "You must enter a subject")]
        [MaxLength(75)]
        public string Subject { get; set; }

        [Required(ErrorMessage = "You must enter a Start Time")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "You must enter an End Time")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "You must enter an appointment Description.")]
        [MaxLength(500)]
        public string Description { get; set; }

        public Vet Vet { get; set; }

        [Display(Name = "Vet")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Vet")]
        public int VetId { get; set; }

        public Pet Pet { get; set; }

        [Display(Name = "Pet")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Pet")]
        public int PetId { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }
    }
}
