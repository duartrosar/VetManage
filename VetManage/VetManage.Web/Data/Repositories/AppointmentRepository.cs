using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboPets()
        {
            var list = _context.Pets
                .Select(p => new SelectListItem
                {
                    Text = $"{p.Name} - {p.Breed}",
                    Value = p.Id.ToString(),
                });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboVets()
        {
            var list = _context.Vets
                .Select(v => new SelectListItem
                {
                    Text = $"{v.FullName}",
                    Value = v.Id.ToString(),
                });

            return list;
        }
    }
}
