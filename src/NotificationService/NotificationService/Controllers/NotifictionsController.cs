using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Data;
using NotificationService.Models;
using Shared.Authentication;

namespace NotificationService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotifictionsController : ControllerBase
    {
        private readonly INotificationRepositroy _notificationRepositroy;
        private readonly IUserSession _session;

        public NotifictionsController(
            INotificationRepositroy notificationRepositroy, 
            IUserSession session
            )
        {
            _notificationRepositroy = notificationRepositroy;
            _session = session;
        }

        [HttpGet]
        [Route("recent")]
        public async Task<List<Notification>> Get(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
            )
        {
            return await _notificationRepositroy
                .GetRecentNotificationsAsync(_session.UserId, pageNumber, pageSize);
        }

        [HttpPost]
        [Route("read")]
        public async Task<IActionResult> Read()
        {
            await _notificationRepositroy.ReadRecentNotificationsAsync(_session.UserId);
            return Ok();
        }
    }
}
