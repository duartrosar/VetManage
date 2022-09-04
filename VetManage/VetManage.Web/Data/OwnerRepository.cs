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
    }
}
