using Business.Services;
using GAMAX.Services.Dto;
using GAMAX.Services.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAcountService _accountService;
        private readonly INotificationServices _notificationServices;
        private readonly HubContextNotify _hubContextNotify;
        public FriendsController(IHttpContextAccessor httpContextAccessor, IAcountService acountService, INotificationServices notificationServices , HubContextNotify hubContextNotify)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountService = acountService;
            _notificationServices = notificationServices;
            _hubContextNotify = hubContextNotify;
        }
        [HttpPost("SearchAccount")]
        public async Task<IActionResult> SearchAccount(string searchString)
        {
            searchString=searchString.ToLower();
            var searchResult = await _accountService.SearchAccountsAsync(searchString);
            return Ok(searchResult);
        }
        [HttpPost("SendFriendRequest")]
        public async Task<IActionResult> SendFriendRequest(Guid userId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var searchResult = await _accountService.SendFriendRequest(userInfo.Uid, userId);
            _notificationServices.NotifyOnSendFriendRequest(userId, userInfo.Uid);
            return Ok(searchResult);
        }
        [HttpPost("GetPendingFriendRequest")]
        public async Task<IActionResult> GetPendingFriendRequest()
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var pendingRequests = await _accountService.GetPendingFriendRequest(userInfo.Uid);
            return Ok(pendingRequests);
        }
        [HttpPost("AproveFriendRequest")]
        public async Task<IActionResult> AproveFriendRequest(Guid RequestId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var searchResult = await _accountService.AproveFriendRequest(RequestId);
            var accountProfile = await _accountService.GetAccountProfileAsync(userInfo.Uid);
            var profileInfo = new DomainModels.DTO.UserAccount
            {
                Id = accountProfile.Id,
                Email = accountProfile.Email,
                UserName = accountProfile.UserName,
                FirstName = accountProfile.FirstName,
                LastName = accountProfile.LastName,
                City = accountProfile.City,
                Country = accountProfile.Country,
                Bio = accountProfile.Bio,
                Birthdate = accountProfile.Birthdate,
                gender = accountProfile.gender.ToString(),
                Type = accountProfile.Type.ToString(),
            };
            await _hubContextNotify.OnApproveFriendRequest(searchResult.Item2, profileInfo);
            return Ok(searchResult.Item1);
        }
        [HttpPost("GetAllUserFriends")]
        public async Task<IActionResult> GetAllUserFreinds(Guid ? userID)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var userFriend = await _accountService.GetAllUserFreinds(userID == null ? userInfo.Uid : (Guid)userID);
            return Ok(userFriend);
        }
        [HttpPost("DeneyFriendRequest")]
        public async Task<IActionResult> DeneyFriendRequest(Guid RequestId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var searchResult = await _accountService.DeneyFriendRequest(RequestId);
            return Ok(searchResult);
        }
        [HttpPost("DeleteFriend")]
        public async Task<IActionResult> DeleteFriend(Guid UserId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var searchResult = await _accountService.DeleteFriend(userInfo.Uid, UserId);
            return Ok(searchResult);
        }
        [HttpPost("GetUserFriendRelation")]
        public async Task<IActionResult> GetUserFriendRelation(Guid SecondUserId)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            var searchResult = await _accountService.GettwoUserFriendRelation(userInfo.Uid, SecondUserId);
            return Ok(searchResult);
        }
    }
}
