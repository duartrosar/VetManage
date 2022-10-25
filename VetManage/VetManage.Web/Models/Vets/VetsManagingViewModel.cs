using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace VetManage.Web.Models.Vets
{
    public class VetsManagingViewModel
    {
        public IEnumerable<SelectListItem> UsersNoEntity { get; set; }

        public ICollection<VetViewModel> Vets { get; set; }

        public VetViewModel VetViewModel { get; set; }

        public RegisterVetViewModel RegisterVetViewModel { get; set; }
    }
}
