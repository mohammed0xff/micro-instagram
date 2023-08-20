using Microsoft.EntityFrameworkCore;
using Shared.Events;
using UserService.Api.Responses;
using UserService.Api.Responses.Enums;
using UserService.DBContext;
using UserService.Entities;
using UserService.Entities.Enums;
using UserService.MessageBus;


namespace UserService.Services
{
    public class UserRepository
    {
        private readonly IMessageBusClient _messageBus;
        public IAppDbContext _dbContext { get; }

        public UserRepository(IAppDbContext dbContext, IMessageBusClient messageBus)
        {
            _dbContext = dbContext;
            _messageBus = messageBus;
        }

        public async Task AddUser(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserResponse?> GetUserByIG(string username)
        {
            try
            {
                var userResponse = await _dbContext.Users
                    .Where(u => u.Username == username)
                    .Select(u => new UserResponse
                    {
                        Username = u.Username,
                        Bio = u.Bio,
                        Email = u.Email,
                        IsPrivate = u.IsPrivate,
                        CreatedDate = u.CreatedDate,
                        // use subquery for FollowStatus
                        FollowStatus = _dbContext.Follows.Any(f => f.FolloweeId == u.Id && !f.IsDeleted)
                            ? FollowStatus.Following
                            : _dbContext.FollowRequests.Any(fr => fr.RecieverId == u.Id && fr.Status == RequestStatus.Pending)
                                ? FollowStatus.FollowRequestSent
                                : FollowStatus.NotFollowing
                    })
                    .FirstOrDefaultAsync();

                return userResponse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<UserResponse>> GetUsers(
            Guid loggedInUserId, int pageNumber = 1, int pageSize = 10
            )
        {
            try
            {
                User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == loggedInUserId);

                if (user == null)
                {
                    throw new Exception("User not found"); // TODO : make specific exceptions `UserNotFoundException`
                }

                var users = await _dbContext.Users
                    .Where(u => u.Id != loggedInUserId)
                    .OrderBy(u => u.Username)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();

                var userIds = users.Select(u => u.Id).ToList();

                var userResponses = users.Select(u => new UserResponse
                {
                    Username = u.Username,
                    Bio = u.Bio,
                    Email = u.Email,
                    IsPrivate = u.IsPrivate,
                    CreatedDate = u.CreatedDate,
                    FollowStatus = _dbContext.Follows.Any(f => f.FolloweeId == u.Id && f.FollowerId == loggedInUserId && !f.IsDeleted)
                                    ? FollowStatus.Following
                                    : _dbContext.FollowRequests.Any(fr => fr.RecieverId == u.Id && fr.SenderId == loggedInUserId && fr.Status == RequestStatus.Pending)
                                    ? FollowStatus.FollowRequestSent
                                    : FollowStatus.NotFollowing
                }).ToList();

                return userResponses;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<UserResponse>> GetFollowers(
            Guid loggedInUserId, string userIG, int pageNumber = 1, int pageSize = 10
            )
        {
            try
            {
                User? user = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == userIG);

                if (user == null)
                {
                    throw new Exception("User not found"); 
                }

                if (loggedInUserId != user.Id && user.IsPrivate && !await IsFollowedBy(user.Id, loggedInUserId))
                {
                    return null; // unauthorized
                }

                // get the IDs of the users who are following user
                var followersIds = _dbContext.Follows
                    .Where(f => f.FolloweeId == user.Id)
                    .Select(f => f.FollowerId);

                // retrieve those users
                var users = await _dbContext.Users
                    .Where(u => followersIds.Contains(u.Id))
                    .Select(u => new UserResponse
                    {
                        Username = u.Username,
                        Bio = u.Bio,
                        Email = u.Email,
                        IsPrivate = u.IsPrivate,
                        CreatedDate = u.CreatedDate,
                        // determine the FollowStatus based on whether the logged in User is following these users
                        FollowStatus = _dbContext.Follows.Any(f => f.FolloweeId == u.Id && f.FollowerId == loggedInUserId && !f.IsDeleted)
                            ? FollowStatus.Following
                            : _dbContext.FollowRequests.Any(fr => fr.RecieverId == u.Id && fr.SenderId == loggedInUserId && fr.Status == RequestStatus.Pending)
                                ? FollowStatus.FollowRequestSent
                                : FollowStatus.NotFollowing
                    })
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<UserResponse>> GetFollowing(
            Guid loggedInUserId, string username, int pageNumber = 1, int pageSize = 10
            )
        {
            try
            {
                User? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

                if (user == null) return null;

                if (loggedInUserId != user.Id && user.IsPrivate && !await IsFollowedBy(user.Id, loggedInUserId))
                {
                    return null;
                }

                // get the IDs of the users who the current user is following
                var followingIds = _dbContext.Follows
                    .Where(f => f.FollowerId == user.Id && !f.IsDeleted)
                    .Select(f => f.FolloweeId);

                // retrieve those users
                var users = await _dbContext.Users
                    .Where(u => followingIds.Contains(u.Id))
                    .Select(u => new UserResponse
                    {
                        Username = u.Username,
                        Bio = u.Bio,
                        Email = u.Email,
                        IsPrivate = u.IsPrivate,
                        CreatedDate = u.CreatedDate,
                        // determine the FollowStatus based on whether the logged in User is following these users
                        FollowStatus = _dbContext.Follows.Any(f => f.FolloweeId == u.Id && f.FollowerId == loggedInUserId && !f.IsDeleted)
                            ? FollowStatus.Following
                            : _dbContext.FollowRequests.Any(fr => fr.RecieverId == u.Id && fr.SenderId == loggedInUserId && fr.Status == RequestStatus.Pending)
                                ? FollowStatus.FollowRequestSent
                                : FollowStatus.NotFollowing
                    })
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();

                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> FollowUser(Guid loggedInUserId, string followeeIG)
        {
            try
            {
                User? follower = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == loggedInUserId);
                User? followee = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == followeeIG);

                if (follower == null || followee == null) { return false; }

                if (followee.IsPrivate) { return false; }

                // Save follow request to database
                var followRequest = new Follow
                {
                    FollowerId = follower.Id,
                    FolloweeId = followee.Id,
                };

                await _dbContext.Follows.AddAsync(followRequest);
                var entries = await _dbContext.SaveChangesAsync();
                if (entries < 1) { return false; }

                // Generate event message for RabbitMQ
                var followEventMessage = new FollowCreatedEvent
                {
                    FollowerId = follower.Id,
                    FollowedId = followee.Id,
                    FollowerUsername = follower.Username
                };

                // Publish event message to RabbitMQ
                _messageBus.PublishFollowEvent(followEventMessage);

                return true;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UnFollowUser(Guid followerIG, string followeeIG)
        {
            try
            {
                User? follower = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == followerIG);
                User? followee = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == followeeIG);

                if (follower == null || followee == null) { return false; }

                Follow? follow = await _dbContext.Follows
                    .AsTracking()
                    .FirstOrDefaultAsync(x =>
                        x.FollowerId == follower.Id && x.FolloweeId == followee.Id && !x.IsDeleted
                        );

                if (follow == null) return false;

                follow.IsDeleted = true;
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SendFollowRequest(Guid loggedInUserId, string RecieverIG)
        {
            User? reciever = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == RecieverIG);
            
            if (reciever == null) { return false; }
            
            User sender = await _dbContext.Users
                .SingleAsync(x => x.Id == loggedInUserId);

            if (!reciever.IsPrivate) return false;
            
            await _dbContext.FollowRequests.AddAsync(
                new FollowRequest { SenderId = sender.Id, RecieverId = reciever.Id }
                );

            var followEventMessage = new FollowRequestCreatedEvent
            {
                FollowerId = sender.Id,
                FollowedId = reciever.Id,
                FollowerUsername = sender.Username
            };

            _messageBus.PublishFollowEvent(followEventMessage);
            
            return true;
        }

        public async Task<bool> CancelFollowRequest(Guid loggedInUserId, string recieverIG)
        {
            try
            {
                User? receiver = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == recieverIG);

                if (receiver == null) { return false; }

                User sender = await _dbContext.Users
                    .SingleAsync(x => x.Id == loggedInUserId);

                var request = await _dbContext.FollowRequests
                    .FirstOrDefaultAsync(f =>
                        f.SenderId == sender.Id && f.RecieverId == receiver.Id
                        );

                if (request is null) return false;

                _dbContext.FollowRequests.Remove(request);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AcceptFollowRequest(Guid loggedInUserId, string senderIG)
        {
            try
            {
                User? sender = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == senderIG);

                if (sender == null) { return false; }

                User reciever = await _dbContext.Users
                    .SingleAsync(x => x.Id == loggedInUserId);

                var request = await _dbContext.FollowRequests.FirstOrDefaultAsync(f =>
                    f.SenderId == sender.Id && f.RecieverId == reciever.Id 
                    );
                
                if (request is null) return false;
                
                _dbContext.FollowRequests.Remove(request);

                await _dbContext.Follows.AddAsync(
                    new Follow() {FollowerId = sender.Id ,FolloweeId = reciever.Id }
                    );
                
                await _dbContext.SaveChangesAsync();

                var followEventMessage = new FollowCreatedEvent
                {
                    FollowerId = reciever.Id,
                    FollowedId = sender.Id

                };

                _messageBus.PublishFollowEvent(followEventMessage);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RejectFollowRequest(Guid loggedInUserId, string senderIG)
        {
            try
            {
                User? reciever = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id == loggedInUserId);
                
                if (reciever == null) { return false; }
                
                User? sender = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Username == senderIG);

                if (sender == null) { return false; }

                var req = await _dbContext.FollowRequests.FirstOrDefaultAsync(
                        x => x.SenderId == sender.Id && x.RecieverId == reciever.Id 
                    );

                if (req == null) return false;
                
                _dbContext.FollowRequests.Remove(req);

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> IsFollowedBy(Guid followee, Guid follower)
        {
            return await _dbContext.Follows
                .AnyAsync(f =>
                    f.FollowerId.Equals(follower) && f.FolloweeId.Equals(followee) && !f.IsDeleted);
        }
    }
}
