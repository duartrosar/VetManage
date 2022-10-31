using VetManage.Web.Data.Entities;

namespace VetManage.Web.Models.Pets
{
    public class PetDetailsViewModel
    {
        public PetViewModel PetViewModel { get; set; }

        public Owner Owner { get; set; }
    }
}
