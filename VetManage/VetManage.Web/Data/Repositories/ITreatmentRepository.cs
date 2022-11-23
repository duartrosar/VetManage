using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface ITreatmentRepository : IGenericRepository<Treatment>
    {
        IQueryable GetAllWithPetsAndSpecialities();

        Task<Treatment> GetWithPetAndSpecialityByIdAsync(int id);
    }
}
