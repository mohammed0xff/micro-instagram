using Microsoft.EntityFrameworkCore;
using UserService.Entities;

namespace UserService.DBContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<FollowRequest> FollowRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(u => u.IsPrivate)
                    .IsRequired();
                entity.Property(u => u.Bio)
                    .HasMaxLength(200);
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(u => u.CreatedDate)
                    .IsRequired();
                entity.Property(u => u.Password)
                    .HasMaxLength(100)
                    .IsRequired();
                
                entity.HasIndex(u => u.Username).IsUnique();
            });

            // Follow entity configuration
            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.IsDeleted).IsRequired();
                entity.Property(f => f.FollowDate).IsRequired();

                entity.HasOne(f => f.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.FollowerId).IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.Followee)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FolloweeId).IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // FollowRequest entity configuration
            modelBuilder.Entity<FollowRequest>(entity =>
            {
                entity.HasKey(fr => fr.Id);
                entity.Property(fr => fr.RequestDate).IsRequired();
                entity.Property(fr => fr.Status).IsRequired();


                // Configure the existing relationship
                entity.HasOne(fr => fr.Follower)
                    .WithMany(u => u.FollowRequestsSent)
                    .HasForeignKey(fr => fr.SenderId).IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(fr => fr.Followee)
                    .WithMany(u => u.FollowRequestsReceived)
                    .HasForeignKey(fr => fr.RecieverId).IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasAlternateKey(fr => new { fr.SenderId, fr.RecieverId });
            });
        }
    }
}
