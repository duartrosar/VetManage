using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VetManage.Web.Models
{
    public class VetsViewModel
    {
        public IEnumerable<SelectListItem> UsersNoEntity { get; set; }

        public ICollection<VetViewModel> Vets { get; set; }

        public VetViewModel Vet { get; set; }
    }
}
