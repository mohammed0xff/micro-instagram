using Xunit;
using Moq;
using MockQueryable.Moq;
using Shared.Events;
using UserService.Api.Responses.Enums;
using UserService.DBContext;
using UserService.Entities;
using UserService.MessageBus;
using UserService.Services;

namespace UserService.Tests
{
    namespace MyProject.Tests
    {
        public class UserRepositoryTests
        {
            private Mock<IAppDbContext> _dbContextMock;
            private Mock<IMessageBusClient> _messageBusClientMock;
            private UserRepository _userRepository;
            
            // source data typically dbsets 
            private List<User> _sourceUsers = new();
            private List<Follow> _sourceFollows = new();
            private List<FollowRequest> _sourceFollowRequests = new();

            // to easily work with users on a certain follow status 
            private List<User> _usersFollowed = new();
            private List<User> _usersUnfollowed = new();
            private List<User> _usersFollowRequested = new();
            private List<User> _usersfollowers = new();

            private User loggedInUser;

            public UserRepositoryTests()
            {
                _dbContextMock = new Mock<IAppDbContext>();
                _messageBusClientMock = new Mock<IMessageBusClient>();
                _userRepository = new UserRepository(_dbContextMock.Object, _messageBusClientMock.Object);
                
                // our dear user
                loggedInUser = new User { Username = "myuser1" };

                // poeple current user is following and other followed by.
                List<Follow> _followers = new();
                List<Follow> _following = new();
                List<FollowRequest> _requestsSent = new();


                _sourceUsers.AddRange(
                    new List<User>()
                    {
                        new User(){ Username = "A", Email = "A"},
                        new User(){ Username = "B", Email = "B"},
                        new User(){ Username = "C", Email = "C"},
                        new User(){ Username = "D", Email = "D"},
                        new User(){ Username = "E", Email = "E"},
                        new User(){ Username = "F", Email = "F"},
                        new User(){ Username = "G", Email = "G"},
                        new User(){ Username = "H", Email = "H"},
                        new User(){ Username = "J", Email = "J"},
                        new User(){ Username = "K", Email = "K"},
                        new User(){ Username = "L", Email = "L"},
                        new User(){ Username = "M", Email = "M"},
                        new User(){ Username = "N", Email = "N"},
                    });

                var mockDbsetUsers = _sourceUsers.AsQueryable().BuildMockDbSet();
                _dbContextMock.Setup(x => x.Users).Returns(mockDbsetUsers.Object);

                // set up follows mock dbset
                _sourceFollows.AddRange(
                    new List<Follow>()
                    {
                        new Follow {FollowerId = _sourceUsers[0].Id, FolloweeId = _sourceUsers[1].Id},
                        new Follow {FollowerId = _sourceUsers[2].Id, FolloweeId = _sourceUsers[3].Id},
                        new Follow {FollowerId = _sourceUsers[4].Id, FolloweeId = _sourceUsers[5].Id},
                    });
                
                _followers.AddRange(
                    new List<Follow>()
                    {
                        new Follow {FollowerId = _sourceUsers[0].Id, FolloweeId = loggedInUser.Id},
                        new Follow {FollowerId = _sourceUsers[1].Id, FolloweeId = loggedInUser.Id},
                        new Follow {FollowerId = _sourceUsers[2].Id, FolloweeId = loggedInUser.Id},
                    });
                
                _following.AddRange(
                    new List<Follow>()
                    {
                        new Follow {FollowerId = loggedInUser.Id, FolloweeId = _sourceUsers[3].Id},
                        new Follow {FollowerId = loggedInUser.Id, FolloweeId = _sourceUsers[4].Id},
                        new Follow {FollowerId = loggedInUser.Id, FolloweeId = _sourceUsers[5].Id},
                    });

                _requestsSent.AddRange(
                    new List<FollowRequest>()
                    {
                        new FollowRequest {SenderId = loggedInUser.Id, RecieverId = _sourceUsers[6].Id},
                        new FollowRequest {SenderId = loggedInUser.Id, RecieverId = _sourceUsers[7].Id},
                        new FollowRequest {SenderId = loggedInUser.Id, RecieverId = _sourceUsers[8].Id},
                    });

                // set up follows mock dbset
                _sourceFollows.AddRange(_followers);
                _sourceFollows.AddRange(_following);
                _sourceFollowRequests.AddRange(_requestsSent);

                var mockDbsetFollows = _sourceFollows.AsQueryable().BuildMockDbSet();
                _dbContextMock.Setup(x => x.Follows).Returns(mockDbsetFollows.Object);

                // set up follow requests mock dbset
                var mockDbsetFollowRequests = _sourceFollowRequests.AsQueryable().BuildMockDbSet();
                _dbContextMock.Setup(x => x.FollowRequests).Returns(mockDbsetFollowRequests.Object);
                _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

                // add our user to the context
                _sourceUsers.Add(loggedInUser);

                // setup mock messageBusClient event publishing
                _messageBusClientMock.Setup(x => x.PublishFollowEvent(It.IsAny<BaseEvent>())).Verifiable();
                _messageBusClientMock.Setup(x => x.PublishUserEvent(It.IsAny<BaseEvent>())).Verifiable();

                // setup working lists
                _usersFollowed = _sourceUsers
                    .Join(_following, x => x.Id, f => f.FolloweeId, (x, f) => x)
                    .ToList();
                
                _usersFollowRequested = _sourceUsers
                    .Join(_requestsSent, x => x.Id, f => f.RecieverId, (x, f) => x)
                    .ToList();
                
                _usersfollowers = _sourceUsers
                    .Join(_followers, x => x.Id, f => f.FollowerId, (x, f) => x)
                    .ToList();
                
                _usersUnfollowed = _sourceUsers.Except(_usersFollowed).ToList();
            }

            [Fact]
            public async void GetUsers_ReturnsUserResponseList()
            {
                // Arrange

                // Act
                var result = await _userRepository.GetUsers(loggedInUser.Id);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                // Excludes the logged-in user
                Assert.Null(result.FirstOrDefault(x => x.Username.Equals(loggedInUser.Username))); 
            }

            [Fact]
            public async void GetUserByIG_ReturnsUserResponse_WhenUserExists()
            {
                // Arrange

                // Act
                var result = await _userRepository.GetUserByIG(loggedInUser.Username);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(result.Username, loggedInUser.Username);
            }

            [Fact]
            public async void GetUserByIG_ReturnsNull_WhenUserDoesntExist()
            {
                // Arrange

                // Act
                var result = await _userRepository.GetUserByIG("nonExistingUsername");

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async void GetAFollowedUser_ReturnsUserResponse_WhenUserExists()
            {
                // Arrange
                User myFollowee = _usersFollowed.First();

                // Act
                var result = await _userRepository.GetUserByIG(myFollowee.Username);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(FollowStatus.Following, result.FollowStatus);
            }

            [Fact]
            public async void GetFollowers_ReturnsFollowerList()
            {
                // Arrange

                // Act
                var result = await _userRepository.GetFollowers(loggedInUser.Id, loggedInUser.Username);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(_usersfollowers.Count, result.Count);
            }

            [Fact]
            public async Task GetFollowers_Throws_WhenUserDoesntExist()
            {
                // Arrange

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () =>
                {
                    await _userRepository.GetFollowers(loggedInUser.Id, "non-existing-username");
                });
            }

            [Fact]
            public async void GetFollowers_ReturnsNull_WhenUserIsPrivate()
            {
                // Arrange
                User privateUser = _usersUnfollowed.First();
                privateUser.IsPrivate = true;

                // Act
                var result = await _userRepository.GetFollowers(loggedInUser.Id, privateUser.Username);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public async void GetFollowers_ReturnsList_WhenUserIsPrivateBUTFollowedBy()
            {
                // Arrange
                User followedPrivateUser = _usersFollowed.First();
                followedPrivateUser.IsPrivate = true;
                
                // Act
                var result = await _userRepository.GetFollowers(loggedInUser.Id, followedPrivateUser.Username);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }


            [Fact]
            public async void GetFollowing_ReturnsFollowingList()
            {
                // Arrange

                // Act
                var result = await _userRepository.GetFollowing(loggedInUser.Id, loggedInUser.Username);

                // Assert
                Assert.Equal(_usersFollowed.Count, result.Count);
                Assert.All(result, x => Assert.Equal(FollowStatus.Following, x.FollowStatus));
            }

            [Fact]
            public async void FollowUser_AddsFollow_WhenNotFollowed()
            {
                // Arrange
                User unfollowedUser = _usersUnfollowed.First();
                
                // Act
                var result = await _userRepository.FollowUser(loggedInUser.Id, unfollowedUser.Username);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public async void FollowUser_ReturnsFalse_WhenUserDoesntExist()
            {
                // Arrange

                // Act
                var result = await _userRepository.FollowUser(loggedInUser.Id, "unexisting-username");

                // Assert
                Assert.False(result);
            }


            [Fact]
            public async void UnFollowUser_ReturnsTrue_WhenFollowExists()
            {
                // Arrange
                User followedUser = _usersFollowed.First();

                // Act
                var result = await _userRepository.UnFollowUser(loggedInUser.Id, followedUser.Username);

                // Assert
                Assert.True(result);
            }


            [Fact]
            public async void SendFollowRequest_ReturnsFalse_NoFollowing()
            {
                // Arrange
                User unfollowedUser = _usersUnfollowed.First();

                // Act
                var result = await _userRepository.UnFollowUser(loggedInUser.Id, unfollowedUser.Username);

                // Assert
                Assert.False(result);
            }

            [Fact]
            public async void SendFollowRequest_ReturnsFalse_WhenUserDoesntExist()
            {
                // Arrange

                // Act
                var result = await _userRepository.SendFollowRequest(loggedInUser.Id, "unexisting-username");

                // Assert
                Assert.False(result);
            }

            [Fact]
            public async void SendFollowRequest_ReturnsFalse_WhenSentAlready()
            {
                // Arrange
                User alreadyRequestedUser = _usersFollowRequested.First();

                // Act
                var result = await _userRepository.SendFollowRequest(loggedInUser.Id, alreadyRequestedUser.Username);

                // Assert
                Assert.False(result);
            }

            [Fact]
            public async void CancelFollowRequest_ReturnsTrue_WhenFollowRequestExists()
            {
                // Arrange
                User requestedUser = _usersFollowRequested.First();
                // Act
                var result = await _userRepository.CancelFollowRequest(loggedInUser.Id, requestedUser.Username);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public async void CancelFollowRequest_ReturnsFalse_WhenFollowRequestDoenstExists()
            {
                // Arrange
                User nonRequestedUser = _usersFollowed.First();

                // Act
                var result = await _userRepository.CancelFollowRequest(loggedInUser.Id, nonRequestedUser.Username);

                // Assert
                Assert.False(result);
            }


            [Fact]
            public async void AcceptFollowRequest_ReturnsTrue_WhenFollowRequestExists()
            {
                // Arrange
                User requestedUser = _usersFollowRequested.First();

                // Act
                var result = await _userRepository.AcceptFollowRequest(requestedUser.Id, loggedInUser.Username);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public async void AcceptFollowRequest_ReturnsFalse_WhenFollowRequestDoenstExists()
            {
                // Arrange
                User nonRequestedUser = _usersUnfollowed.First();

                // Act
                var result = await _userRepository.AcceptFollowRequest(nonRequestedUser.Id, loggedInUser.Username);

                // Assert
                Assert.False(result);
            }

            [Fact]
            public async void RejectFollowRequest_ReturnsTrue_WhenFollowRequestExists()
            {
                // Arrange
                User requestedUser = _usersFollowRequested.First();
                // Act
                var result = await _userRepository.RejectFollowRequest(requestedUser.Id, loggedInUser.Username);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public async void RejectFollowRequest_ReturnsFalse_WhenFollowRequestDoenstExists()
            {
                // Arrange
                User nonRequestedUser = _usersUnfollowed.First();

                // Act

                var result = await _userRepository.RejectFollowRequest(nonRequestedUser.Id, loggedInUser.Username);

                // Assert
                Assert.False(result);
            }

        }
    }
}