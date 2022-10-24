using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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

        // TO DO: Make sure user has access to certain common properties of the various Entities
        //public IEntity UserEntity { get; set; }

        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";

        // Save the path to the user's profile image, this will be the same as its entity
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return $"https://localhost:44318/images/noimage.png"; ;
                }

                return $"https://localhost:44318{ImageUrl.Substring(1)}";
            }
        }
    }
}
