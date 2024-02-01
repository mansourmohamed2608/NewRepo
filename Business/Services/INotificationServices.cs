using DataBase.Core.Enums;
using DomainModels.DTO;
using DomainModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface INotificationServices
    {
        Task<bool> AddNotification(NotificationDTO notification);
        Task NotifyOnAddingComment(CommentDTO commentDTO, Guid postId, PostsTypes postsType);
        Task NotifyOnAddingPost(PostDTO postDTO);
        Task NotifyOnAddingQuestion(QuestionPostDTO postDTO);
        Task NotifyOnAddingReactPost(ReactsDTO reactDTO , AddReactRequest reactRequest);
        Task NotifyOnAddingReactQuestionPost(ReactsDTO reactDTO, AddReactRequest reactRequest);
        Task NotifyOnAddingReactOnComment(ReactsDTO reactDTO, AddReactRequest reactRequest);
        Task NotifyOnAddingReactOnAnswer(ReactsDTO reactDTO, AddReactRequest reactRequest);
        Task<bool> NotifyOnApproveFriendRequest(Guid ApprovedUserId, Guid userId);
        Task NotifyOnSendFriendRequest(Guid RecivedUserId, Guid userId);
        Task<bool> RemoveAllUserNotification(Guid Id);
        Task<IEnumerable<NotificationDTO>> GetAllUserNotifications(Guid userId);
    }
}
