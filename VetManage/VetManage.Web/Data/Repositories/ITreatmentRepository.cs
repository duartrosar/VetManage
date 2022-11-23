using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface ITreatmentRepository : IGenericRepository<Treatment>
    {
        IQueryable GetAllWithPetsAndSpecialities();

        IQueryable GetAllByPetId(int petId);

        IQueryable GetAllByOwnerId(int ownerId);

        IQueryable GetLatestTreatments();

        Task<Treatment> GetWithPetAndSpecialityByIdAsync(int id);
        IQueryable GetLatestTreatmentsByOwnerId(int id);
    }
}
