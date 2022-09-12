using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
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

        public IEnumerable<SelectListItem> GetComboUsers()
        {
            // Get all the users with the role Client as list of SelectListItem
            var list = _context.Users
                .Where(u => u.RoleName == "Client")
                .Select(u => new SelectListItem
                {
                    Text = u.FullName,
                    Value = u.Id.ToString(),
                }).ToList();

            // Insert a item as the first element of the list to serve as a placeholder
            // And help guarantee the user selects a role
            //list.Insert(0, new SelectListItem
            //{
            //    Text = "<Select a user>",
            //    Value = ""
            //});

            return list;
        }
    }
}
