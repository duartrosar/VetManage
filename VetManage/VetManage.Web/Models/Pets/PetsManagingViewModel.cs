using System.Collections.Generic;
using VetManage.Web.Models.Owners;

namespace VetManage.Web.Models.Pets
{
    public class PetsManagingViewModel
    {
        public ICollection<OwnerViewModel> Owners { get; set; }

        public ICollection<PetViewModel> Pets { get; set; }

        public PetViewModel PetViewModel { get; set; }
    }
}
