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

        public IEnumerable<SelectListItem> GetComboUsersNoEntity()
        {
            // Get all the users with the role Client that do not have an entity associated with them as list of SelectListItem
            var list = _context.Users
                .Where(u => u.RoleName == "Client")
                .Where(u => !u.HasEntity)
                .Select(u => new SelectListItem
                {
                    Text = u.FullName,
                    Value = u.Id.ToString(),
                }).ToList();

            return list;
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

        public IEnumerable<SelectListItem> GetComboUsers()
        {
            // Get all the users with the role Client as a list of SelectListItem
            var list = _context.Users
                .Where(u => u.RoleName == "Client")
                .Select(u => new SelectListItem
                {
                    Text = u.FullName,
                    Value = u.Id.ToString(),
                }).ToList();

            return list;
        }

        public async Task<Owner> GetByUserIdAsync(User user)
        {
            return await _context.Owners
                .FirstOrDefaultAsync(v => v.User == user);
        }
    }
}
