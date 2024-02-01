using Business.Services;
using GAMAX.Services.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationController(INotificationServices notificationServices, IHttpContextAccessor httpContextAccessor)
        {
            _notificationServices = notificationServices;
            _httpContextAccessor = httpContextAccessor;

        }
        [HttpPost("GetUserNotifications")]
        public async Task<IActionResult> GetProfileAcountData()
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var notifications = await _notificationServices.GetAllUserNotifications(userInfo.Uid);
            return Ok(notifications);
        }
        [HttpPost("MarkUserNotificationsAsRead")]
        public async Task<IActionResult> MarkUserNotificationsAsRead()
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            await _notificationServices.RemoveAllUserNotification(userInfo.Uid);
            return Ok();
        }
    }
}

