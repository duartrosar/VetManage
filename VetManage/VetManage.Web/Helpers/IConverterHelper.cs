﻿using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models.Calendar;
using VetManage.Web.Models.Messages;
using VetManage.Web.Models.Owners;
using VetManage.Web.Models.Pets;
using VetManage.Web.Models.Vets;

namespace VetManage.Web.Helpers
{
    public interface IConverterHelper
    {
        Owner ToOwner(OwnerViewModel model, bool isNew, string path);
        OwnerViewModel ToOwnerViewModel(Owner owner);
        ICollection<OwnerViewModel> AllToOwnerViewModel(IQueryable owners);


        Pet ToPet(PetViewModel model, bool isNew, string path);
        PetViewModel ToPetViewModel(Pet pet);
        ICollection<PetViewModel> AllToPetViewModel(IQueryable pets);


        Vet ToVet(VetViewModel model, bool isNew, string path);
        VetViewModel ToVetViewModel(Vet vet);
        ICollection<VetViewModel> AllToVetViewModel(IQueryable vets);


        Appointment ToAppointment(AppointmentViewModel model, bool isNew);
        AppointmentViewModel ToAppointmentViewModel(Appointment appointment);
        ICollection<AppointmentViewModel> AllToAppointmentViewModel(IQueryable appointments);


        Message ToMessage(MessageViewModel model);
        MessageViewModel ToMessageViewModel(Message message, MessageMessageBox mmb);

        MessageViewModel ToMessageViewModelOutbox(Message message);

        ICollection<MessageViewModel> AllToMessageViewModel(IQueryable<Message> messages, IQueryable<MessageMessageBox> messageMessageBoxes);
        ICollection<MessageViewModel> AllToMessageViewModelOutbox(IQueryable<Message> outbox);
    }
}
