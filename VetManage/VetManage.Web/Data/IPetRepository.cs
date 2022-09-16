using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
{
    public interface IPetRepository : IGenericRepository<Pet>
    {
        IQueryable GetAllWithOwners();
    }
}
