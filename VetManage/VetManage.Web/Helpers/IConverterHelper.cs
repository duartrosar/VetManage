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
    }
}
