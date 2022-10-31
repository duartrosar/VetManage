using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface IPetRepository : IGenericRepository<Pet>
    {
        IQueryable GetAllWithOwners();

        Task<Pet> GetWithOwnerByIdAsync(int id);
    }
}
