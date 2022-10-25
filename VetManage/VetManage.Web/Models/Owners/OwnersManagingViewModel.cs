using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Owners
{
    public class OwnersManagingViewModel
    {
        public ICollection<OwnerViewModel> Owners { get; set; }

        public OwnerViewModel OwnerViewModel { get; set; }

        public RegisterOwnerViewModel RegisterOwnerViewModel { get; set; }
    }
}
