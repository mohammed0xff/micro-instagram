using Microsoft.EntityFrameworkCore;
using UserService.Entities;

namespace UserService.DBContext
{
    public interface IAppDbContext : IAsyncDisposable, IDisposable
    {
        DbSet<FollowRequest> FollowRequests { get; }
        DbSet<Follow> Follows { get; }
        DbSet<User> Users { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}