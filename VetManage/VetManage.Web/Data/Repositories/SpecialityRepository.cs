using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class SpecialityRepository : GenericRepository<Speciality>, ISpecialityRepository
    {
        private readonly DataContext _context;

        public SpecialityRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboSpecialities()
        {
            var list = _context.Specialities.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            }).OrderBy(s => s.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Select a Speciality",
                Value = "0",
            });

            return list;
        }

        public IQueryable GetLatestSpecialities()
        {
            return _context.Specialities
                .OrderByDescending(t => t.Id)
                .Take(3);
        }
    }
}
