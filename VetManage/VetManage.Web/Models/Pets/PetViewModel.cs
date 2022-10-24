using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Pets
{
    public class PetViewModel : Pet
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}
