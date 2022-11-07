using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment> GetWithMembersByIdAsync(int id);

        IEnumerable<SelectListItem> GetComboPets();

        IEnumerable<SelectListItem> GetComboVets();
    }
}
