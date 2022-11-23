using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models.Account;
using VetManage.Web.Models.Calendar;
using VetManage.Web.Models.Messages;
using VetManage.Web.Models.Owners;
using VetManage.Web.Models.Pets;
using VetManage.Web.Models.Specialities;
using VetManage.Web.Models.Treatments;
using VetManage.Web.Models.Users;
using VetManage.Web.Models.Vets;

namespace VetManage.Web.Helpers
{
    public interface IConverterHelper
    {
        Owner ToOwner(OwnerViewModel model, bool isNew, Guid imageId);
        OwnerViewModel ToOwnerViewModel(Owner owner);
        Owner EditProfileViewModelToOwner(EditProfileViewModel model, Owner owner, Guid imageId);
        ICollection<OwnerViewModel> AllToOwnerViewModel(IQueryable owners);


        Pet ToPet(PetViewModel model, bool isNew, Guid imageId);
        PetViewModel ToPetViewModel(Pet pet);
        ICollection<PetViewModel> AllToPetViewModel(IQueryable pets);


        Vet ToVet(VetViewModel model, bool isNew, Guid imageId);
        VetViewModel ToVetViewModel(Vet vet);
        Vet EditProfileViewModelToVet(EditProfileViewModel model, Vet vet, Guid imageId);
        ICollection<VetViewModel> AllToVetViewModel(IQueryable vets);


        Appointment ToAppointment(AppointmentViewModel model, bool isNew);
        AppointmentViewModel ToAppointmentViewModel(Appointment appointment);
        ICollection<AppointmentViewModel> AllToAppointmentViewModel(IQueryable appointments);


        Message ToMessage(MessageViewModel model);
        MessageViewModel ToMessageViewModel(Message message);
        MessageViewModel ToMessageViewModel(Message message, MessageMessageBox mmb);
        MessageViewModel ToMessageViewModelOutbox(Message message);
        ICollection<MessageViewModel> AllToMessageViewModel(IQueryable<Message> messages, IQueryable<MessageMessageBox> messageMessageBoxes);
        ICollection<MessageViewModel> AllToMessageViewModelOutbox(IQueryable<Message> outbox);
        ICollection<MessageViewModel> AllToMessageViewModel(IQueryable messages);


        User ToUser(IIsUser entity, User user, string blobContainerName);
        User EditProfileViewModelToUser(EditProfileViewModel model, User user, string blobContainerName);
        User ToUser(User user, UserViewModel model, bool isNew, string blobContainerName);
        UserViewModel ToUserViewModel(User user);


        Speciality ToSpeciality(SpecialityViewModel model, bool isNew);
        SpecialityViewModel ToSpecialityViewModel(Speciality speciality);
        ICollection<SpecialityViewModel> AllToSpecialityViewModel(IQueryable specialities);

        Treatment ToTreatment(TreatmentViewModel model, bool isNew);
        TreatmentViewModel ToTreatmentViewModel(Treatment treatment);
        ICollection<TreatmentViewModel> AllToTreatmentViewModel(IQueryable treatments);

        List<SelectListItem> GetGenders();
        List<SelectListItem> GetPetGenders();
    }
}
