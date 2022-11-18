using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Vets
{
    public class VetViewModel : Vet
    {
        [Display(Name = "User")]
        //[Required(ErrorMessage = "You must choose a User")]
        public string UserId { get; set; }

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public string ImageIdString { get; set; }

        public bool IsAdmin { get; set; }
    }
}
