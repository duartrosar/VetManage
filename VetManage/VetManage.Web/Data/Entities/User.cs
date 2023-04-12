using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetManage.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "You must enter a First Name")]
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "You must enter a Last Name")]
        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "You must enter a First Name")]
        [MaxLength(250)]
        public string Address { get; set; }

        [Required(ErrorMessage = "You must select a First Name")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "You must select a Date Of Birth")]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        // TO DO: Make sure user has access to certain common properties of the various Entities
        //public IEntity UserEntity { get; set; }

        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";

        // Keep record if user has changed its password before the first login
        public bool PasswordChanged { get; set; }

        public MessageBox MessageBox { get; set; }

        public string BlobContainer { get; set; }


        List<Appointment> Appointments { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
             ? $"https://vetmanage.azurewebsites.net/images/noimage.png"
             : $"https://vetmanagestorage.blob.core.windows.net/{BlobContainer}/{ImageId}";
    }
}
