using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VetManage.Web.Models
{
    public class RegisterNewUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string Address { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }


        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }


        [Display(Name = "Role")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a country")]
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
