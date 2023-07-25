
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
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message)
                .HasMaxLength(120)
                .IsRequired();
                entity.HasAlternateKey(e => new {e.SenderId, e.ReceiverId});
                entity.ToTable("Notifications");
            });
        }
    }
}
