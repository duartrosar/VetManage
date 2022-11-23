using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        IQueryable GetAllByOwnerId(int id);

        IQueryable GetAllWithMembers();

        IQueryable GetAllFromToday();

        IQueryable GetAllByOwnerIdFromToday(int id);

        IQueryable GetMostRecentlyBooked();

        Task<Appointment> GetWithMembersByIdAsync(int id);

        IEnumerable<SelectListItem> GetComboPets();

        IEnumerable<SelectListItem> GetComboVets();

        IQueryable GetMostRecentlyBookedByOwnerId(int id);
    }
}
