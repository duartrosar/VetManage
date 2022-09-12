using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models
{
    public class OwnersViewModel
    {
        public IEnumerable<SelectListItem> Users { get; set; }

        public ICollection<OwnerViewModel> Owners { get; set; }

        public OwnerViewModel Owner { get; set; }
    }
}
