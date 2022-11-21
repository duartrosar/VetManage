using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Users
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "You must enter an Email")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required(ErrorMessage = "You must enter a First Name")]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You must enter a Last Name ")]
        [MaxLength(50)]
        [Display(Name = "Surname")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter a Gender")]
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

        [Required]
        public bool IsAdmin { get; set; }

        public string BlobContainer { get; set; }


        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://vetmanage.azurewebsites.net/images/nouser.png"
            : $"https://vetmanagestorage.blob.core.windows.net/{BlobContainer}/{ImageId}";

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
