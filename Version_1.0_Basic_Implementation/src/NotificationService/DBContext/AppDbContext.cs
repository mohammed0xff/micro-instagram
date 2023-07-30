
using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Notifications entity
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.SenderId)
                    .IsRequired();

                entity.Property(n => n.ReceiverId)
                    .IsRequired();

                entity.Property(n => n.Message)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(n => n.CreatedAt)
                    .IsRequired();

                entity.HasIndex(n => n.ReadAt);

                entity.ToTable("Notifications");
            });
        }
    }
}
