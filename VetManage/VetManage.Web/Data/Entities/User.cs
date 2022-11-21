using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetManage.Web.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string Address { get; set; }

        /// <summary>
        /// RoleId is the id used by the dropdown to access the correct role
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// RoleName is used to insert the user into a role and to search
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// The id of the Entity associated with the
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Whether or not the user has an Entity associated with it
        /// </summary>
        public bool HasEntity { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "DOB")]
        public DateTime DateOfBirth { get; set; }

        // TO DO: Make sure user has access to certain common properties of the various Entities
        //public IEntity UserEntity { get; set; }

        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";

        // Keep record if user has changed its password before the first login
        public bool PasswordChanged { get; set; }

        public MessageBox MessageBox { get; set; }

        public string BlobContainer { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
             ? $"https://vetmanage.azurewebsites.net/images/noimage.png"
             : $"https://vetmanagestorage.blob.core.windows.net/{BlobContainer}/{ImageId}";
    }
}
