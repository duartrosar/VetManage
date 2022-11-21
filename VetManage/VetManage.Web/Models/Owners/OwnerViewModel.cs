using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Owners
{
    public class OwnerViewModel : Owner
    {

        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        public string ImageIdString { get; set; }
    }
}
