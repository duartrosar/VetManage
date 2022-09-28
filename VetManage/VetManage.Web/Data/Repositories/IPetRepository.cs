using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface IPetRepository : IGenericRepository<Pet>
    {
        IQueryable GetAllWithOwners();
    }
}
