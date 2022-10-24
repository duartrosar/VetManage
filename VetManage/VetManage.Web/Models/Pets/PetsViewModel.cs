using System.Collections.Generic;
using VetManage.Web.Models.Owners;

namespace VetManage.Web.Models.Pets
{
    public class PetsViewModel
    {
        public ICollection<OwnerViewModel> Owners { get; set; }

        public ICollection<PetViewModel> Pets { get; set; }

        public PetViewModel Pet { get; set; }
    }
}
