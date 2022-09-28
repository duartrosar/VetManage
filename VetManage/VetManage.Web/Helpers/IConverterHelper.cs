using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models;

namespace VetManage.Web.Helpers
{
    public interface IConverterHelper
    {
        Owner ToOwner(OwnerViewModel model, bool isNew);
        OwnerViewModel ToOwnerViewModel(Owner owner);
        ICollection<OwnerViewModel> AllToOwnerViewModel(IQueryable owners);


        Pet ToPet(PetViewModel model, bool isNew);
        PetViewModel ToPetViewModel(Pet pet);
        ICollection<PetViewModel> AllToPetViewModel(IQueryable pets);


        Vet ToVet(VetViewModel model, bool isNew);
        VetViewModel ToVetViewModel(Vet vet);
        ICollection<VetViewModel> AllToVetViewModel(IQueryable vets);


        Appointment ToAppointment(AppointmentViewModel model, bool isNew);
        AppointmentViewModel ToAppointmentViewModel(Appointment appointment);
        ICollection<AppointmentViewModel> AllToAppointmentViewModel(IQueryable appointments);
    }
}
