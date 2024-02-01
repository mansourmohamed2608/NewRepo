using Business;
using DataBase.Core.Enums;
using DataBase.Core.Models;
using DomainModels.DTO;
using Microsoft.AspNetCore.SignalR;

namespace GAMAX.Services.Hubs
{
    public class HubContextNotify
    {
        private readonly IHubContext<SingalRHub> _hubContext;
        private readonly UserConnectionManager _userConnectionManager;
        private readonly SignalRActions _signalRActions;
        public HubContextNotify(IHubContext<SingalRHub> hubContext , UserConnectionManager userConnectionManager , SignalRActions signalRActions)
        {
            _hubContext = hubContext;
            _userConnectionManager = userConnectionManager;
            _signalRActions = signalRActions;
            _signalRActions.OnAddingPostAction = OnAddingPost;
            _signalRActions.OnAddingCommentAction = OnAddingComment;
            _signalRActions.OnAddingReactOnPostAction = OnAddingReactOnPost;
            _signalRActions.OnSendingFriendRequestAction = OnSendFriendRequest;
            _signalRActions.OnApprovedFriendRequestAction = OnApproveFriendRequest;
            _signalRActions.OnAddingReactOnCommentAction = OnAddingReactOnComment;
        }
        public async Task ReceiveMessage(Guid RecivedUserId, ChatDTO chatDTO)
        {
            var connID = _userConnectionManager.GetUserConnection(RecivedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("ReceiveMessage", chatDTO);
            }
        }
        public async Task UpdateMessage(Guid RecivedUserId, ChatDTO chatDTO)
        {
            var connID = _userConnectionManager.GetUserConnection(RecivedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("UpdateMessage", chatDTO);
            }
        }
        public async Task DeleteMessage(Guid RecivedUserId, Chat chat)
        {
            var connID = _userConnectionManager.GetUserConnection(RecivedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("DeleteMessage", chat);
            }
        }
        public async Task Hello(Guid ApprovedUserId)
        {
            var connID = _userConnectionManager.GetUserConnection(ApprovedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("ApproveFriend", "Hello");
            }
        }
        public async Task OnSendFriendRequest(Guid RecivedUserId, FriendRequestUserAccount userAccount)
        {
            var connID = _userConnectionManager.GetUserConnection(RecivedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("OnSendFriendRequest", userAccount);
            }
          }
        public async Task OnApproveFriendRequest(Guid ApprovedUserId, UserAccount userAccount)
        {
            var connID = _userConnectionManager.GetUserConnection(ApprovedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("OnApproveFriendRequest", userAccount);
            }
        }
        public async Task OnAddingPost(NotificationModel notification)
        {
            await _hubContext.Clients.All.SendAsync("OnAddingPostOrQuestion", notification);
        }
        public async Task OnAddingComment(NotificationModel notification)
        {
            var connID = _userConnectionManager.GetUserConnection(notification.NotifiedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("OnAddingComment", notification);
            }
        }
        public async Task OnAddingReactOnPost(NotificationModel notification)
        {
            var connID = _userConnectionManager.GetUserConnection(notification.NotifiedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("OnAddingReactOnPost", notification);
            }
        }
        public async Task OnAddingReactOnComment(NotificationModel notification)
        {
            var connID = _userConnectionManager.GetUserConnection(notification.NotifiedUserId);
            if (!string.IsNullOrEmpty(connID))
            {
                await _hubContext.Clients.Client(connID).SendAsync("OnAddingReactOnComment", notification);
            }
        }
    }
}
