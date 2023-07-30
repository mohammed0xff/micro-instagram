using UserService.Entities;
using UserService.Entities.Enums;

namespace UserService.DBContext.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(this IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                await SeedDataAsync(serviceScope);
            }
        }

        public static async Task SeedDataAsync(IServiceScope serviceScope)
        {
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            
            context.Database.EnsureCreated();

            #region SeedUsers

            if (context.Users.Any()) return;

            var password = "$2a$11$UImimJXg6.sKudoofSzxCO9rw8DlelGzrNLwyaOqMikyvj3sJe5QS"; // value : string

            // Create more users
            var user1 = new User { Username = "User1", Email = "user1@example.com" , Password = password };
            var user2 = new User { Username = "User2", Email = "user2@example.com", IsPrivate = true , Password = password };
            var user3 = new User { Username = "User3", Email = "user3@example.com" , Password = password };
            var user4 = new User { Username = "User4", Email = "user4@example.com" , Password = password };
            var user5 = new User { Username = "User5", Email = "user5@example.com", IsPrivate = true , Password = password };

            context.Users.AddRange(user1, user2, user3, user4, user5);

            await context.SaveChangesAsync();

            // Create more follows
            var follow1 = new Follow { FollowerId = user1.Id, FolloweeId = user2.Id };
            var follow2 = new Follow { FollowerId = user2.Id, FolloweeId = user3.Id };
            var follow3 = new Follow { FollowerId = user3.Id, FolloweeId = user1.Id };
            var follow4 = new Follow { FollowerId = user4.Id, FolloweeId = user1.Id };
            var follow5 = new Follow { FollowerId = user5.Id, FolloweeId = user4.Id };

            context.Follows.AddRange(follow1, follow2, follow3, follow4, follow5);

            await context.SaveChangesAsync();

            // Create more follow requests
            var followRequest1 = new FollowRequest { SenderId = user1.Id, RecieverId = user3.Id, Status = RequestStatus.Pending };
            var followRequest2 = new FollowRequest { SenderId = user2.Id, RecieverId = user1.Id, Status = RequestStatus.Pending };
            var followRequest3 = new FollowRequest { SenderId = user4.Id, RecieverId = user2.Id, Status = RequestStatus.Pending };
            var followRequest4 = new FollowRequest { SenderId = user5.Id, RecieverId = user3.Id, Status = RequestStatus.Pending };

            context.FollowRequests.AddRange(followRequest1, followRequest2, followRequest3, followRequest4);

            await context.SaveChangesAsync();

            #endregion

        }
    }
}