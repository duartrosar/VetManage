using System.Collections.Generic;
using VetManage.Web.Models.Specialities;

namespace VetManage.Web.Models.Home
{
    public class HomeViewModel
    {
        public IEnumerable<SpecialityViewModel> LastestSpecialities { get; set; }
    }
}
