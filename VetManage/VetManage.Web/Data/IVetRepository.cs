using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
{
    public interface IVetRepository : IGenericRepository<Vet>
    {
        IQueryable GetAllWithUsers();

        Task<Vet> GetWithUserByIdAsync(int id);

        IEnumerable<SelectListItem> GetComboUsersNoEntity();

        IEnumerable<SelectListItem> GetComboUsers();
    }
}
