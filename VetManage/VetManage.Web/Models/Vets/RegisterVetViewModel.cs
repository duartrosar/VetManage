using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Vets
{
    public class RegisterVetViewModel
    {
        public VetViewModel VetViewModel { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
    }
}
