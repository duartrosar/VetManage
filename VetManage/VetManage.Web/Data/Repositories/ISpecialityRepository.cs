using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public interface ISpecialityRepository : IGenericRepository<Speciality>
    {
        public IEnumerable<SelectListItem> GetComboSpecialities();

        public IQueryable GetLatestSpecialities();
    }
}
