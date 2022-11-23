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

        public DbSet<Speciality> Specialities { get; set; }

        public DbSet<Treatment> Treatments { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>(builder =>
            {
                builder.HasOne(m => m.Sender)
                .WithMany(mb => mb.Inbox)
                .OnDelete(DeleteBehavior.Restrict);
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

            modelBuilder.Entity<Owner>(builder =>
            {
                builder.HasMany(o => o.Pets)
                .WithOne(p => p.Owner)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasOne(u => u.MessageBox)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Vet>(builder =>
            {
                builder.HasOne(v => v.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Owner>(builder =>
            {
                builder.HasOne(o => o.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Appointment>(builder =>
            {
                builder.HasOne(a => a.Pet)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(a => a.Vet)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Treatment>(builder =>
            {
                builder.HasOne(t => t.Speciality)
                .WithMany(s => s.Treatments)
                .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(t => t.Pet)
                .WithMany(p => p.Treatments)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
