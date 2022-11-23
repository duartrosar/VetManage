using HtmlAgilityPack;
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

        public Owner EditProfileViewModelToOwner(EditProfileViewModel model, Owner owner, Guid imageId)
        {
            owner.FirstName = model.FirstName;
            owner.LastName = model.LastName;
            owner.DateOfBirth = model.DateOfBirth;
            owner.Gender = model.Gender;
            owner.Address = model.Address;
            owner.MobileNumber = model.MobileNumber;
            owner.ImageId = imageId;

            return owner;
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
        public Vet EditProfileViewModelToVet(EditProfileViewModel model, Vet vet, Guid imageId)
        {
            vet.FirstName = model.FirstName;
            vet.LastName = model.LastName;
            vet.DateOfBirth = model.DateOfBirth;
            vet.Gender = model.Gender;
            vet.Address = model.Address;
            vet.MobileNumber = model.MobileNumber;
            vet.ImageId = imageId;

            return vet;
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
                // TODO: Recipients = model.RecipientsList, 
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



        public User ToUser(IIsUser entity, User user, string blobContainerName)
        {
            user.FirstName = entity.FirstName;
            user.LastName = entity.LastName;
            user.Address = entity.Address;
            user.PhoneNumber = entity.MobileNumber;
            user.ImageId = entity.ImageId;
            user.Gender = entity.Gender;
            user.DateOfBirth = entity.DateOfBirth;
            user.BlobContainer = blobContainerName;

            return user;
        }

        public User EditProfileViewModelToUser(EditProfileViewModel model, User user, string blobContainerName)
        {
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.MobileNumber;
            user.ImageId = model.ImageId;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.BlobContainer = blobContainerName;

            return user;
        }

        public User ToUser(User user, UserViewModel model, bool isNew, string blobContainerName)
        {
            user.Id = isNew ? Guid.Empty.ToString() : model.Id;
            user.UserName = model.Username;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.MobileNumber;
            user.ImageId = model.ImageId;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.BlobContainer = blobContainerName;

            return user;
        }

        public UserViewModel ToUserViewModel(User user)
        {
            return new UserViewModel
            {
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                MobileNumber = user.PhoneNumber,
                ImageId = user.ImageId,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                BlobContainer = user.BlobContainer,
            };
        }

        public SpecialityViewModel ToSpecialityViewModel(Speciality speciality)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(speciality.Description);
            string bodyRaw = htmlDoc.DocumentNode.InnerText;

            bodyRaw = bodyRaw.Replace("&nbsp;", Environment.NewLine);

            return new SpecialityViewModel
            {
                Id = speciality.Id,
                Name = speciality.Name,
                Description = speciality.Description,
                DescriptionRaw = bodyRaw,
                DescriptionAbbreviation = bodyRaw.Substring(0, Math.Min(bodyRaw.Length - 1, 190)) + "...",
            };
        }

        public Speciality ToSpeciality(SpecialityViewModel model, bool isNew)
        {
            return new Speciality
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Description = model.Description,
            };
        }

        public ICollection<SpecialityViewModel> AllToSpecialityViewModel(IQueryable specialities)
        {
            List<SpecialityViewModel> specialityViewModels = new List<SpecialityViewModel>();

            foreach (Speciality speciality in specialities)
            {
                specialityViewModels.Add(ToSpecialityViewModel(speciality));
            }

            return specialityViewModels;
        }

        public Treatment ToTreatment(TreatmentViewModel model, bool isNew)
        {
            return new Treatment
            {
                Id = isNew ? 0 : model.Id,
                Notes = model.Notes,
                PetId = model.PetId,
                SpecialityId = model.SpecialityId,
                TreatmentDate = model.TreatmentDate,
            };
        }

        public TreatmentViewModel ToTreatmentViewModel(Treatment treatment)
        {
            return new TreatmentViewModel
            {
                Id = treatment.Id,
                Notes = treatment.Notes,
                TreatmentDate = treatment.TreatmentDate,
                Pet = treatment.Pet,
                Speciality = treatment.Speciality,
                PetId = treatment.PetId,
                SpecialityId = treatment.SpecialityId,
                DateString = treatment.TreatmentDate.ToShortDateString(),
            };
        }

        public ICollection<TreatmentViewModel> AllToTreatmentViewModel(IQueryable treatments)
        {
            List<TreatmentViewModel> treatmentViewModels = new List<TreatmentViewModel>();

            foreach(Treatment treatment in treatments)
            {
                treatmentViewModels.Add(ToTreatmentViewModel(treatment));
            }

            return treatmentViewModels;
        }
    }
}
