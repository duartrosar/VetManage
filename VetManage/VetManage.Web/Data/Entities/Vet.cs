using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Vet : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Surname")]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "DOB")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        [MaxLength(250)]
        public string Address { get; set; }

        public User User { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://vetmanage.azurewebsites.net/images/noimage.png"
            : $"https://vetmanagestorage.blob.core.windows.net/vets/{ImageId}";
    }
}
