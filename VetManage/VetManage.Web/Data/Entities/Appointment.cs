using System;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Helpers;

namespace VetManage.Web.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required(ErrorMessage = "You must enter a subject")]
        public string Subject { get; set; }

        [Required]
        [PastDate(ErrorMessage = "Can't select dates in the past.")]
        public DateTime StartTime { get; set; }

        [Required]
        [PastDate(ErrorMessage = "Can't select dates in the past.")]
        public DateTime EndTime { get; set; }

        [Required]
        public string Description { get; set; }

        public Vet Vet { get; set; }

        public int VetId { get; set; }

        public Pet Pet { get; set; }

        public int PetId { get; set; }
    }
}
