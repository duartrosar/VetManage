using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data.Repositories
{
    public class SpecialitiesRepository : GenericRepository<Speciality>, ISpecialitiesRepository
    {
        public SpecialitiesRepository(DataContext context) : base(context)
        {
        }
    }
}
