using System.Collections.Generic;

namespace VetManage.Web.Models
{
    public class PetsViewModel
    {
        public ICollection<OwnerViewModel> Owners { get; set; }

        public ICollection<PetViewModel> Pets { get; set; }

        public PetViewModel Pet { get; set; }
    }
}
