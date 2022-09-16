using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models;

namespace VetManage.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        //////// OWNERS ////////
        public Owner ToOwner(OwnerViewModel model, bool isNew)
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
        public Pet ToPet(PetViewModel model, bool isNew)
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
                Owner = model.Owner,
                OwnerId = model.OwnerId,
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
                Owner = pet.Owner,
                OwnerId = pet.OwnerId,
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
        public Vet ToVet(VetViewModel model, bool isNew)
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
    }
}
