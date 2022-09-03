using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Owner> Owners { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
