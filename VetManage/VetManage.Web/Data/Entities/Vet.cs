using System;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Data.Entities
{
    public class Vet : IEntity, IIsUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a First Name")]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You must enter a Last Name ")]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must select a Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "You must enter a Date Of Birth")]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "You must enter a Mobile Number")]
        [MaxLength(20)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "You must enter an Address")]
        [MaxLength(250)]
        public string Address { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://vetmanage.azurewebsites.net/images/nouser.png"
            : $"https://vetmanagestorage.blob.core.windows.net/vets/{ImageId}";
    }
}
