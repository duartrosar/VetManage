using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models;

namespace VetManage.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        //////// OWNERS ////////
        public Owner ToOwner(OwnerViewModel model, bool isNew, string path)
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
                ImageUrl = path
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
                ImageUrl = owner.ImageUrl
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
        public Pet ToPet(PetViewModel model, bool isNew, string path)
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
                ImageUrl = path,
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
                ImageUrl = pet.ImageUrl,
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
        public Vet ToVet(VetViewModel model, bool isNew, string path)
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
                ImageUrl = path,
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
                ImageUrl = vet.ImageUrl
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
    }
}
