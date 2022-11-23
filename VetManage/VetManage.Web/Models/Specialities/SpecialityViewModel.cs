using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Specialities
{
    public class SpecialityViewModel : Speciality
    {
        public string DescriptionRaw { get; set; }

        public string DescriptionAbbreviation { get; set; }
    }
}
