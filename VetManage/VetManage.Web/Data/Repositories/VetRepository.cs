using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class VetRepository : GenericRepository<Vet>, IVetRepository
    {
        private readonly DataContext _context;

        public VetRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Vets
                .Include(v => v.User)
                .OrderBy(v => v.FirstName)
                .ThenBy(v => v.LastName);
        }

        public async Task<Vet> GetWithUserByIdAsync(int id)
        {
            return await _context.Vets
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vet> GetByUserIdAsync(User user)
        {
            return await _context.Vets
                .FirstOrDefaultAsync(v => v.User == user);
        }
    }
}
