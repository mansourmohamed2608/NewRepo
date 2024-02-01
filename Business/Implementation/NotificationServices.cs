using Business.Services;
using DataBase.Core;
using DataBase.Core.Enums;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.Posts;
using DomainModels;
using DomainModels.DTO;
using DomainModels.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilites;

namespace Business.Implementation
{
    public class NotificationServices : INotificationServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignalRActions _signalRActions;
        public NotificationServices(IUnitOfWork unitOfWork, SignalRActions signalRActions)
        {
            _unitOfWork = unitOfWork;
            _signalRActions = signalRActions;
        }
        public async Task<bool> AddNotification(NotificationDTO notification)
        {
            var _notification = new Notifications
            {
                Id = Guid.NewGuid(),
                ActionedUserId = notification.ActionedUserId,
                ItemId = notification.ItemId,
                NotificatinType = notification.NotificatinType,
                NotifiedUserId = notification.NotifiedUserId,
            };
             await _unitOfWork.Notification.AddAsync(_notification);
             return await _unitOfWork.Complete() >0;
        }
        public async Task<IEnumerable<NotificationDTO>> GetAllUserNotifications(Guid userId)
        {
            string[] includes = { "ActionedUser" };
            var notifications = await _unitOfWork.Notification.FindAllAsync(n => n.NotifiedUserId == userId, includes);
            var notificationDTO = OMapper.Mapper.Map<List<DomainModels.DTO.NotificationDTO>>(notifications);
            return notificationDTO;
        }
        public Task NotifyOnAddingComment(CommentDTO commentDTO, Guid postId, PostsTypes postsType)
        {
            Guid userPostOwnerId;
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = commentDTO.UserId,
                ActionUserFirstName = commentDTO.UserFirstName,
                ActionUserLastName = commentDTO.UserLastName,
                ItemId = postId,
            };
            if (postsType == PostsTypes.Post)
            {
                notificationDTO.NotificatinType = NotificatinTypes.AddComment;
                userPostOwnerId = _unitOfWork.Post.Find(p => p.Id == postId).UserAccountsId;
                notificationDTO.NotifiedUserId=userPostOwnerId;
            }
            else
            {
                notificationDTO.NotificatinType = NotificatinTypes.AddAnswer;
                userPostOwnerId = _unitOfWork.QuestionPost.Find(p => p.Id == postId).UserAccountsId;
                notificationDTO.NotifiedUserId = userPostOwnerId;
            }
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = notificationDTO.ActionedUserId,
                ActionUserFirstName = notificationDTO.ActionUserFirstName,
                ActionUserLastName = notificationDTO.ActionUserLastName,
                NotifiedUserId = userPostOwnerId,
                TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                PostId = postId,
                PostsType = postsType,
                NotificatinType = notificationDTO.NotificatinType,
            };

            AddNotification(notificationDTO);
            Task.Run(async() => {
                
                await SendCommentNotification(notificationModel);
            });

            return Task.CompletedTask;
        }
        public Task NotifyOnAddingPost(PostDTO postDTO)
        {
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                ItemId = postDTO.Id,
                NotificatinType = NotificatinTypes.AddPost
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                NotifiedUserId = postDTO.UserId,
                TimeCreated= postDTO.TimeCreated,
                PostId = postDTO.Id,
                PostsType=PostsTypes.Post,
                NotificatinType = notificationDTO.NotificatinType,
            };
            Task.Run(async () => {  await SendPostNotification(notificationModel); });
            return Task.CompletedTask;
        }
        public Task NotifyOnAddingQuestion(QuestionPostDTO postDTO)
        {
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                ItemId = postDTO.Id,
                NotificatinType = NotificatinTypes.AddQuestion
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = postDTO.UserId,
                ActionUserFirstName = postDTO.UserFirstName,
                ActionUserLastName = postDTO.UserLastName,
                NotifiedUserId = postDTO.UserId,
                TimeCreated = postDTO.TimeCreated,
                PostId = postDTO.Id,
                PostsType = PostsTypes.Question,
                NotificatinType = notificationDTO.NotificatinType,
            };
            Task.Run(async () => {await SendPostNotification(notificationModel); });
            return Task.CompletedTask;
        }
        public async Task<bool> RemoveAllUserNotification(Guid Id)
        {
            var notifications =  await _unitOfWork.Notification.FindAllAsync(n => n.NotifiedUserId == Id);
            if (notifications != null)
            {
                _unitOfWork.Notification.DeleteRange(notifications);
                 return await _unitOfWork.Complete()>0;
            }
            return true;
        }
        public async Task<bool> NotifyOnApproveFriendRequest(Guid RequestorUserId, Guid approvedUserId)
        {
            var accountProfile =  _unitOfWork.UserAccounts.Find(p => p.Id == approvedUserId);
            var userAccount = new DomainModels.DTO.UserAccount
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
            await ApproveFriendRequestNotification(RequestorUserId, userAccount);
            //Task.Run(async () => {
                
            //});
            return true;
        }
        public Task NotifyOnSendFriendRequest(Guid RecivedUserId, Guid userId)
        {
            var requestNotification = GetFriendRequestData(RecivedUserId, userId);
            Task.Run(async () => {
                await SendFriendRequestNotification(RecivedUserId, requestNotification);
            });
            return Task.CompletedTask; 
        }
        public Task NotifyOnAddingReactPost(ReactsDTO reactDTO, AddReactRequest reactRequest)
        {
            var post = _unitOfWork.Post.Find(p => p.Id == reactRequest.ObjectId);
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = reactDTO.UserId,
                ActionUserFirstName = reactDTO.UserFirstName,
                ActionUserLastName = reactDTO.UserLastName,
                ItemId = post.Id,
                NotificatinType = NotificatinTypes.AddReactOnPost,
                NotifiedUserId = post.UserAccountsId,
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = reactDTO.UserId,
                ActionUserFirstName = reactDTO.UserFirstName,
                ActionUserLastName = reactDTO.UserLastName,
                NotifiedUserId = post.UserAccountsId,
                TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                PostId = reactRequest.ObjectId,
                PostsType = PostsTypes.Post,
                NotificatinType = notificationDTO.NotificatinType,
            };
            notificationDTO.NotifiedUserId = notificationModel.NotifiedUserId;

            AddNotification(notificationDTO);
            Task.Run(async () => {
                
                await SendReactNotificationOnPost(notificationModel);
            });
            return Task.CompletedTask;
        }
        public Task NotifyOnAddingReactQuestionPost(ReactsDTO reactDTO, AddReactRequest reactRequest)
        {
            var question = _unitOfWork.QuestionPost.Find(p => p.Id == reactRequest.ObjectId);
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = reactDTO.UserId,
                ActionUserFirstName = reactDTO.UserFirstName,
                ActionUserLastName = reactDTO.UserLastName,
                ItemId = question.Id,
                NotificatinType = NotificatinTypes.AddReactOnQuestion,
                NotifiedUserId = question.UserAccountsId,
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = reactDTO.UserId,
                ActionUserFirstName = reactDTO.UserFirstName,
                ActionUserLastName = reactDTO.UserLastName,
                NotifiedUserId = question.UserAccountsId,
                TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                PostId = reactRequest.ObjectId,
                PostsType = PostsTypes.Question,
                NotificatinType = notificationDTO.NotificatinType,
            };
            AddNotification(notificationDTO);
            Task.Run(async () =>
            {
                await SendReactNotificationOnPost(notificationModel);
            });
            return Task.CompletedTask;
        }
        public Task NotifyOnAddingReactOnComment(ReactsDTO reactDTO, AddReactRequest reactRequest)
        {
            var commment = _unitOfWork.PostComment.Find(c => c.Id == reactRequest.ObjectId);
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = reactDTO.UserId,
                ActionUserFirstName = reactDTO.UserFirstName,
                ActionUserLastName = reactDTO.UserLastName,
                ItemId = commment.PostId,
                NotificatinType = NotificatinTypes.AddReactOnComment,
                NotifiedUserId = commment.UserAccountsId,
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = notificationDTO.ActionedUserId,
                ActionUserFirstName = notificationDTO.ActionUserFirstName,
                ActionUserLastName = notificationDTO.ActionUserLastName,
                NotifiedUserId = notificationDTO.NotifiedUserId,
                TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                PostId = commment.PostId,
                PostsType = PostsTypes.Post,
                NotificatinType = notificationDTO.NotificatinType,
            };
            AddNotification(notificationDTO);
            Task.Run(async () =>
            {
                await SendReactNotificationOnComment(notificationModel);
            });
            return Task.CompletedTask;
        }
        public Task NotifyOnAddingReactOnAnswer(ReactsDTO reactDTO, AddReactRequest reactRequest)
        {
            var commment = _unitOfWork.QuestionComment.Find(c => c.Id == reactRequest.ObjectId);
            var notificationDTO = new NotificationDTO()
            {
                ActionedUserId = reactDTO.UserId,
                ActionUserFirstName = reactDTO.UserFirstName,
                ActionUserLastName = reactDTO.UserLastName,
                ItemId = commment.QuestionPostId,
                NotificatinType = NotificatinTypes.AddReactOnAnswer,
                NotifiedUserId = commment.UserAccountsId,
            };
            var notificationModel = new DomainModels.DTO.NotificationModel
            {
                ActionedUserId = notificationDTO.ActionedUserId,
                ActionUserFirstName = notificationDTO.ActionUserFirstName,
                ActionUserLastName = notificationDTO.ActionUserLastName,
                NotifiedUserId = notificationDTO.NotifiedUserId,
                TimeCreated = TimeHelper.ConvertTimeCreateToString(DateTime.UtcNow),
                PostId = commment.QuestionPostId,
                PostsType = PostsTypes.Question,
                NotificatinType = notificationDTO.NotificatinType,
            };
            AddNotification(notificationDTO);
            Task.Run(async () =>
            {
                await SendReactNotificationOnComment(notificationModel);
            });
            return Task.CompletedTask;
        }
        private FriendRequestUserAccount GetFriendRequestData(Guid RecivedUserId, Guid userId)
        {
            string[] includes = { "Requestor" };
            var pendingList =  _unitOfWork.FriendRequests.FindAll(f => f.ReceiverId == RecivedUserId && f.RequestorId==userId, includes).FirstOrDefault();
            var userAccounts = OMapper.Mapper.Map<DomainModels.DTO.FriendRequestUserAccount>(pendingList);
            return userAccounts;
        }
        private async Task SendPostNotification(DomainModels.DTO.NotificationModel notification)
        {
            await _signalRActions.OnAddingPostAction?.Invoke(notification);
        }
        private async Task SendCommentNotification(DomainModels.DTO.NotificationModel notification)
        {
            await _signalRActions.OnAddingCommentAction?.Invoke(notification);
        }
        private async Task SendReactNotificationOnPost(DomainModels.DTO.NotificationModel notification)
        {
            await _signalRActions.OnAddingReactOnPostAction?.Invoke(notification);
        }
        private async Task SendReactNotificationOnComment(DomainModels.DTO.NotificationModel notification)
        {
            await _signalRActions.OnAddingReactOnCommentAction?.Invoke(notification);
        }
        private async Task SendFriendRequestNotification(Guid RecivedUserId, FriendRequestUserAccount userAccount)
        {
            await _signalRActions.OnSendingFriendRequestAction?.Invoke(RecivedUserId, userAccount);
        }
        private async Task ApproveFriendRequestNotification(Guid RequstorUserId, UserAccount userAccount)
        {
            await _signalRActions.OnApprovedFriendRequestAction?.Invoke(RequstorUserId, userAccount);
        }
        private Guid GetPostownerIdFromComment(Guid commentId , PostsTypes postsType)
        {
            switch (postsType)
            {
                case PostsTypes.Post:
                    var postId = _unitOfWork.PostComment.Find(c => c.Id == commentId).PostId;
                    return _unitOfWork.Post.Find(p=>p.Id== postId).UserAccountsId;
                case PostsTypes.Question:
                    var QuestionId = _unitOfWork.QuestionComment.Find(c => c.Id == commentId).QuestionPostId;
                    return _unitOfWork.QuestionPost.Find(p => p.Id == QuestionId).UserAccountsId;
                default:break;
            }
            return default;
        }

        
    }
}
