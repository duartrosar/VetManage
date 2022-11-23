using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Task<Appointment> GetWithMembersByIdAsync(int id)
        {
            return _context.Appointments
                .Include(a => a.Vet)
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);
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

        public IQueryable GetAllByOwnerId(int id)
        {
            return _context.Appointments
                .Where(a => a.Pet.OwnerId == id);
        }

        public IQueryable GetAllWithMembers()
        {
            return _context.Appointments
                .Include(_a => _a.Vet)
                .Include(_a => _a.Pet);
        }

        public IQueryable GetAllFromToday()
        {
            return _context.Appointments
                .Include(a => a.Vet)
                .Include(a => a.Pet)
                .Where(a => a.StartTime.Date == DateTime.Now.Date)
                .OrderBy(a => a.StartTime)
                .Take(3);
        }

        public IQueryable GetMostRecentlyBooked()
        {
            return _context.Appointments
                .Include(a => a.Vet)
                .Include(a => a.Pet)
                .OrderByDescending(a => a.Id)
                .Take(3);
        }

        public IQueryable GetMostRecentlyBookedByOwnerId(int id)
        {
            return _context.Appointments
                .Include(a => a.Vet)
                .Include(a => a.Pet)
                .Where(a => a.Pet.OwnerId == id)
                .OrderByDescending(a => a.Id)
                .Take(3);
        }

        public IQueryable GetAllByOwnerIdFromToday(int id)
        {
            return _context.Appointments
                .Include(a => a.Vet)
                .Include(a => a.Pet)
                .Where(a => a.Pet.OwnerId == id)
                .Where(a => a.StartTime.Date == DateTime.Now.Date);
        }
    }
}
