using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models.Calendar;
using VetManage.Web.Models.Messages;
using VetManage.Web.Models.Owners;
using VetManage.Web.Models.Pets;
using VetManage.Web.Models.Vets;

namespace VetManage.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        //////// OWNERS ////////
        public Owner ToOwner(OwnerViewModel model, bool isNew, Guid imageId)
        {
            return new Owner
            {
                Id = isNew ? 0 : model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                MobileNumber = model.MobileNumber,
                Pets = model.Pets,
                User = model.User,
                ImageId = imageId,
            };
        }

        public OwnerViewModel ToOwnerViewModel(Owner owner)
        {
            return new OwnerViewModel
            {
                Id = owner.Id,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                DateOfBirth = owner.DateOfBirth,
                Gender = owner.Gender,
                Address = owner.Address,
                MobileNumber = owner.MobileNumber,
                Pets = owner.Pets,
                User = owner.User,
                UserId = owner.User.Id,
                ImageId = owner.ImageId
            };
        }


        public ICollection<OwnerViewModel> AllToOwnerViewModel(IQueryable owners)
        {
            List<OwnerViewModel> ownerViewModels = new List<OwnerViewModel>();

            foreach(Owner owner in owners)
            {
                ownerViewModels.Add(ToOwnerViewModel(owner));
            }

            return ownerViewModels;
        }



        //////// PETS ////////
        public Pet ToPet(PetViewModel model, bool isNew, Guid imageId)
        {
            return new Pet
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Breed = model.Breed,
                Type = model.Type,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Height = model.Height,
                Weight = model.Weight,
                Length = model.Length,
                OwnerId = model.OwnerId,
                ImageId = imageId,
            };
        }

        public PetViewModel ToPetViewModel(Pet pet)
        {
            return new PetViewModel
            {
                Id = pet.Id,
                Name = pet.Name,
                Breed = pet.Breed,
                Type = pet.Type,
                DateOfBirth = pet.DateOfBirth,
                Gender = pet.Gender,
                Height = pet.Height,
                Weight = pet.Weight,
                Length = pet.Length,
                OwnerId = pet.OwnerId,
                Owner = pet.Owner,
                ImageId = pet.ImageId,
            };
        }

        public ICollection<PetViewModel> AllToPetViewModel(IQueryable pets)
        {
            List<PetViewModel> petViewModels = new List<PetViewModel>();

            foreach(Pet pet in pets)
            {
                petViewModels.Add(ToPetViewModel(pet));
            };

            return petViewModels;
        }



        //////// VETS ////////
        public Vet ToVet(VetViewModel model, bool isNew, Guid imageId)
        {
            return new Vet
            {
                Id = isNew ? 0 : model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                MobileNumber = model.MobileNumber,
                User = model.User,
                ImageId = imageId,
            };
        }

        public VetViewModel ToVetViewModel(Vet vet)
        {
            return new VetViewModel
            {
                Id = vet.Id,
                FirstName = vet.FirstName,
                LastName = vet.LastName,
                DateOfBirth = vet.DateOfBirth,
                Gender = vet.Gender,
                Address = vet.Address,
                MobileNumber = vet.MobileNumber,
                User = vet.User,
                UserId = vet.User.Id,
                ImageId = vet.ImageId
            };
        }

        public ICollection<VetViewModel> AllToVetViewModel(IQueryable vets)
        {
            List<VetViewModel> vetViewModels = new List<VetViewModel>();

            foreach (Vet vet in vets)
            {
                vetViewModels.Add(ToVetViewModel(vet));
            }

            return vetViewModels;
        }


        //////// APPOINTMETS ////////
        public Appointment ToAppointment(AppointmentViewModel model, bool isNew)
        {
            // Get the correct start and time and add them to the DateTime objects
            TimeSpan startTime = TimeSpan.Parse(model.StartTimeString);
            TimeSpan endTime = TimeSpan.Parse(model.EndTimeString);

            model.StartTime = model.StartTime.Date + startTime;
            model.EndTime = model.StartTime.Date + endTime;

            return new Appointment
            {
                Id = isNew ? 0 : model.Id,
                Title = model.Title,
                Subject = model.Subject,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Description = model.Description,
                VetId = model.VetId,
                PetId = model.PetId
            };
        }

        public AppointmentViewModel ToAppointmentViewModel(Appointment appointment)
        {
            return new AppointmentViewModel
            {
                Id = appointment.Id,
                Title = appointment.Title,
                Subject = appointment.Subject,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Description = appointment.Description,
                Vet = appointment.Vet,
                Pet = appointment.Pet,
                VetId = appointment.VetId,
                PetId = appointment.PetId
            };
        }

        public ICollection<AppointmentViewModel> AllToAppointmentViewModel(IQueryable appointments)
        {
            List<AppointmentViewModel> appointmentViewModels = new List<AppointmentViewModel>();

            foreach(Appointment appointment in appointments)
            {
                appointmentViewModels.Add(ToAppointmentViewModel(appointment));
            }

            return appointmentViewModels;
        }

        public Message ToMessage(MessageViewModel model)
        {
            return new Message
            {
                Body = model.Body,
                Subject = model.Subject,
                //Recipients = model.RecipientsList, // Todo
            };
        }

        public MessageViewModel ToMessageViewModel(Message message, MessageMessageBox mmb)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(message.Body);
            string bodyRaw = htmlDoc.DocumentNode.InnerText;

            return new MessageViewModel
            {
                Id = mmb.MessageId,
                SenderName = message.Sender.User.FullName,
                MessageBoxId = mmb.MessageBoxId,
                Body = message.Body,
                BodyRaw = bodyRaw,
                Subject = message.Subject,
                DateString = message.Date.ToShortDateString(),
                IsRead = mmb.IsRead,
            };
        }

        public MessageViewModel ToMessageViewModelOutbox(Message message)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(message.Body);
            string bodyRaw = htmlDoc.DocumentNode.InnerText;

            return new MessageViewModel
            {
                Id = message.Id,
                SenderName = message.Sender.User.FullName,
                Body = message.Body,
                BodyRaw = bodyRaw,
                Subject = message.Subject,
                DateString = message.Date.ToShortDateString(),
                IsRead = true,
            };
        }

        public ICollection<MessageViewModel> AllToMessageViewModel(IQueryable<Message> messages, IQueryable<MessageMessageBox> messageMessageBoxes)
        {
            List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
            List<Message> messageList = messages.ToList();

            int index = 0;

            foreach (MessageMessageBox mmb in messageMessageBoxes)
            {
                messageViewModels.Add(ToMessageViewModel(messageList[index], mmb));
                index++;
            }

            return messageViewModels;
        }

        public ICollection<MessageViewModel> AllToMessageViewModelOutbox(IQueryable<Message> messages)
        {
            List<MessageViewModel> messageViewModels = new List<MessageViewModel>();
            List<Message> messageList = messages.ToList();

            foreach (Message message in messageList)
            {
                messageViewModels.Add(ToMessageViewModelOutbox(message));
            }

            return messageViewModels;
        }
    }
}
