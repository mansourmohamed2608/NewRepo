using Business;
using DataBase.Core.Enums;
using DataBase.Core.Models;
using DomainModels.DTO;
using GAMAX.Services.Dto;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace GAMAX.Services.Hubs
{
    public class SingalRHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserConnectionManager _userConnectionManager;
        //
        public SingalRHub(IHttpContextAccessor httpContextAccessor, UserConnectionManager userConnectionManager /*, HubContextNotify hubContextNotify*/)
        {
            _httpContextAccessor = httpContextAccessor;
            _userConnectionManager = userConnectionManager;
            //_hubContextNotify = hubContextNotify;
            
        }
        public override Task OnConnectedAsync()
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            string connectionId = Context.ConnectionId;
            _userConnectionManager.AddUserConnection(userInfo.Uid, connectionId);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            _userConnectionManager.RemoveUserConnection(userInfo.Uid);
            return base.OnDisconnectedAsync(exception);
        }
        public async Task TypeMessage( Guid UsersenderID,Guid RecivedUserId)
        {
            var connRecivedID = _userConnectionManager.GetUserConnection(RecivedUserId);
            var connSenderID = _userConnectionManager.GetUserConnection(UsersenderID);
            if (!string.IsNullOrEmpty(connRecivedID))
            {
                await Clients.Client(connRecivedID).SendAsync("typing");
            }
            //if (!string.IsNullOrEmpty(connSenderID))
            //{
            //    await Clients.Client(connSenderID).SendAsync("typing");
            //}
        }
    }
}
