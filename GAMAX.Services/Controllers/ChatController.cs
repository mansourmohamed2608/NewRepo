using Business.Services;
using DomainModels.DTO;
using GAMAX.Services.Dto;
using GAMAX.Services.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ChatController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatServices _chatServices ;
        private readonly HubContextNotify _hubContextNotify;
        public ChatController(IHttpContextAccessor httpContextAccessor , IChatServices chatServices , HubContextNotify hubContextNotify)
        {
            _hubContextNotify = hubContextNotify;
            _httpContextAccessor = httpContextAccessor;
            _chatServices = chatServices;
        }
        [HttpPost("SendPrivateMessage")]
        public async Task<IActionResult> SearchAccount([FromForm] UploadChatDTO uploadChatDTO)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _chatServices.SendPrivateMessage(uploadChatDTO);
            if(result.Item1)
                await _hubContextNotify.ReceiveMessage(result.Item2.ReciveId, result.Item2);
            return Ok(result.Item2);
        }
        [HttpPost("GetUserChat")]
        public async Task<IActionResult> GetUserChat(Guid secoundUserId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _chatServices.GetUserChat(userInfo.Uid ,secoundUserId);
            return Ok(result);
        }
        [HttpPost("MarkUserChatAsRead")]
        public async Task<IActionResult> MarkUserChatAsRead(Guid secoundUserId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _chatServices.MarkUserChatAsRead(userInfo.Uid, secoundUserId);
            return Ok(result);
        }
        [HttpPost("GetFriendsWithLastMessage")]
        public async Task<IActionResult> GetFriendsWithLastMessage()
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _chatServices.GetFriendsWithLastMessage(userInfo.Uid);
            return Ok(result);
        }
        [HttpPost("DeleteChat")]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _chatServices.DeleteChat(userInfo.Uid , chatId);
            await _hubContextNotify.DeleteMessage(result.Item2.ReciveId, result.Item2);
            return Ok(result.Item1);
        }
        [HttpPost("UpdateChat")]
        public async Task<IActionResult> UpdateChat([FromForm] UpdateChatDTO updateChatDTO)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var result = await _chatServices.UpdateChat(updateChatDTO, userInfo.Uid);
            await _hubContextNotify.UpdateMessage(result.Item2.ReciveId, result.Item2);
            return Ok(result.Item2);
        }
    }
}
