using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class VetViewModel : Vet
    {
        public IEnumerable<SelectListItem> Users { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "You must choose a User")]
        public string UserId { get; set; }
    }
}
