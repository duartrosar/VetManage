using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class OwnerViewModel : Owner
    {
        public IEnumerable<SelectListItem> Users { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "You must choose a User")]
        public string UserId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
