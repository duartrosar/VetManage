using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Account
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string Address { get; set; }


        [MaxLength(20, ErrorMessage = "The field {0} only can contain {1} characters length.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Role")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a role")]
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
