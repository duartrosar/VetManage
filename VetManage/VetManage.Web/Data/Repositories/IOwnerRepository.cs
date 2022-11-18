using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface IOwnerRepository : IGenericRepository<Owner>
    {
        IQueryable GetAllWithUsers();

        Task<Owner> GetWithUserByIdAsync(int id);

        IEnumerable<SelectListItem> GetComboUsers();

        IQueryable GetAllWithPetsAndUsers();

        IEnumerable<SelectListItem> GetComboUsersNoEntity();

        Task<Owner> GetByUserIdAsync(User user);
    }
}
