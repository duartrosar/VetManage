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

        public IQueryable GetAllWithPetsAndSpecialities()
        {
            return _context.Treatments
                .Include(t => t.Pet)
                .Include(t => t.Speciality);
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
