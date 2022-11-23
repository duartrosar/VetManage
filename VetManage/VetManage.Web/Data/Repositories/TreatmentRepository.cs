using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class TreatmentRepository : GenericRepository<Treatment> , ITreatmentRepository
    {
        private readonly DataContext _context;

        public TreatmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllByOwnerId(int ownerId)
        {
            return _context.Treatments
                .Include(t => t.Pet)
                .Include(t => t.Speciality)
                .Where(t => t.Pet.OwnerId == ownerId);
        }

        public IQueryable GetAllByPetId(int petId)
        {
            return _context.Treatments
                //.Include(t => t.Pet)
                //.Include(t => t.Speciality)
                .Where(t => t.PetId == petId);
        }

        public IQueryable GetAllWithPetsAndSpecialities()
        {
            return _context.Treatments
                .Include(t => t.Pet)
                .Include(t => t.Speciality);
        }

        public IQueryable GetLatestTreatments()
        {
            return _context.Treatments
                .Include(t => t.Pet)
                .Include(t => t.Speciality)
                .OrderByDescending(t => t.Id)
                .Take(3);
        }

        public IQueryable GetLatestTreatmentsByOwnerId(int id)
        {
            return _context.Treatments
                .Include(t => t.Pet)
                .Include(t => t.Speciality)
                .Where(t => t.Pet.OwnerId == id)
                .OrderByDescending(t => t.Id)
                .Take(3);
        }

        public Task<Treatment> GetWithPetAndSpecialityByIdAsync(int id)
        {
            return _context.Treatments
                .Include(t => t.Pet)
                .Include(t => t.Speciality)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
