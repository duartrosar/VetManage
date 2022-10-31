using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {
        private readonly DataContext _context;

        public PetRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithOwners()
        {
            return _context.Pets.Include(p => p.Owner);
        }

        public async Task<Pet> GetWithOwnerByIdAsync(int id)
        {
            return await _context.Pets
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
