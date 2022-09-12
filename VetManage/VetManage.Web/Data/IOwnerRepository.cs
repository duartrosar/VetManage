using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
{
    public interface IOwnerRepository : IGenericRepository<Owner>
    {
        IQueryable GetAllWithUsers();

        IEnumerable<SelectListItem> GetComboUsers();
    }
}
