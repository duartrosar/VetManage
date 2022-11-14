using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VetManage.Web.Data.Entities;

namespace VetManage.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Owner> Owners { get; set; }

        public DbSet<Vet> Vets { get; set; }

        public DbSet<Pet> Pets { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<MessageBox> MessageBoxes { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<MessageMessageBox> MessageMessageBox { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>(builder =>
            {
                builder.HasOne(m => m.Sender).WithMany(mb => mb.Inbox).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<MessageMessageBox>(builder =>
            {
                builder.HasKey(mmb => new { mmb.MessageId, mmb.MessageBoxId });

                builder.HasOne(mmb => mmb.Message)
                .WithMany(m => m.Recipients)
                .HasForeignKey(mmb => mmb.MessageId);

                builder.HasOne(mmb => mmb.MessageBox)
                .WithMany(mb => mb.Outbox)
                .HasForeignKey(mmb => mmb.MessageBoxId);
            });

            //modelBuilder.Entity<MessageBox>(builder =>
            //{
            //    builder.HasMany(mb => mb.Inbox).WithOne(m => m.Sender).OnDelete(DeleteBehavior.Restrict);
            //});
        }
    }
}
