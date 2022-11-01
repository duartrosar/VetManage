using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models.Owners;

namespace VetManage.Web.Models.Pets
{
    public class PetViewModel : Pet
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
