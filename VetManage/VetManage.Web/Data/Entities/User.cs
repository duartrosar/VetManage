﻿using Microsoft.AspNetCore.Identity;
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

        public string RoleName { get; set; }

        // TO DO: Make sure user has access to certain common properties of the various Entities
        //public IEntity UserEntity { get; set; }

        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
