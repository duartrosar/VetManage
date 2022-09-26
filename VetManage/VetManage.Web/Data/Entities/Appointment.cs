using System;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Helpers;

namespace VetManage.Web.Data.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }

        [Required]
        [PastDate(ErrorMessage = "Can't select dates in the past.")]
        public DateTime StartTime { get; set; }

        [Required]
        [PastDate(ErrorMessage = "Can't select dates in the past.")]
        public DateTime EndTime { get; set; }

        [Required]
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public string ThemeColor { get; set; }
    }
}
