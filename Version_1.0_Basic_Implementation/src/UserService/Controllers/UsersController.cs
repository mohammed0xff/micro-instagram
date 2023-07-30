using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Authentication;
using UserService.Api.Responses;
using UserService.Entities;
using UserService.Services;

namespace UserService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly IUserSession _session;

        public UsersController(UserRepository UserRepository, IUserSession session)
        {
            userRepository = UserRepository;
            _session = session;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserResponse>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            try
            {
                return await userRepository.GetUsers(_session.UserId, pageNumber, pageSize);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{username}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByIG([FromRoute] string username)
        {
            try
            {
                var user = await userRepository.GetUserByIG(username);
                return user is null ? NotFound() : Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/follow")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Follow([FromRoute] string username)
        {
            try
            {
                return await userRepository.FollowUser(_session.UserId, username)
                    ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/unfollow")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> UnFollow([FromRoute] string username)
        {
            try
            {
                return await userRepository.UnFollowUser(_session.UserId, username)
                    ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/send-follow-request")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendFollowRequest([FromRoute] string username)
        {
            try
            {
                return await userRepository.SendFollowRequest(_session.UserId, username)
                     ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/cancel-follow-request")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFollowRequest([FromRoute] string username)
        {
            try
            {
                return await userRepository.RejectFollowRequest(_session.UserId, username)
                    ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/accept-follow-request")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> AcceptFollowRequest([FromRoute] string username)
        {
            try
            {
                return await userRepository.AcceptFollowRequest(_session.UserId, username)
                    ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/reject-follow-request")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectFollowRequest([FromRoute] string username)
        {
            try
            {
                return await userRepository.RejectFollowRequest(_session.UserId, username)
                    ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/followers")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Followers([FromRoute] string username)
        {
            try
            {
                var res = await userRepository.GetFollowers(_session.UserId, username);
                return res is null ? BadRequest() : Ok(res);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{username}/following")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Following([FromRoute] string username)
        {
            try
            {
                var res = await userRepository.GetFollowing(_session.UserId, username);
                return res is null ? BadRequest() : Ok(res);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
