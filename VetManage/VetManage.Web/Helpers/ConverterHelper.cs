using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;
using VetManage.Web.Models;

namespace VetManage.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
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
    }
}
