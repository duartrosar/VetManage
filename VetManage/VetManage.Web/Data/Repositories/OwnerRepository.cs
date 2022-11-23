using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class OwnerRepository : GenericRepository<Owner>, IOwnerRepository
    {
        private readonly DataContext _context;

        public OwnerRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Owners.Include(o => o.User);
        }

        public async Task<Owner> GetWithUserByIdAsync(int id)
        {
            return await _context.Owners
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public IQueryable GetAllWithPetsAndUsers()
        {
            return _context.Owners.Include(o => o.User).Include(o => o.Pets);
        }


        public async Task<Owner> GetByUserIdAsync(string id)
        {
            return await _context.Owners
                .FirstOrDefaultAsync(v => v.User.Id == id);
        }

        public async Task<Owner> GetByUserIdWithPetsAsync(string id)
        {
            return await _context.Owners
                .Include(o => o.Pets)
                .FirstOrDefaultAsync(v => v.User.Id == id);
        }
    }
}
