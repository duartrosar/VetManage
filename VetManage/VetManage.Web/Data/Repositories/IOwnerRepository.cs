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
        IQueryable GetAllWithPetsAndUsers();
        Task<Owner> GetByUserIdAsync(string id);
        Task<Owner> GetByUserIdWithPetsAsync(string id);
    }
}
