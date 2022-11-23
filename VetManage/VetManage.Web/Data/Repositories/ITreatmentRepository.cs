using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface ITreatmentRepository : IGenericRepository<Treatment>
    {
        IQueryable GetAllWithPetsAndSpecialities();

        IQueryable GetAllByPetId(int petId);

        Task<Treatment> GetWithPetAndSpecialityByIdAsync(int id);
    }
}
