using BDataBase.Core.Models.Accounts;
using DataBase.Core.Enums;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.Posts;
using DomainModels.DTO;

namespace Business
{
    public class SignalRActions
    {
        public SignalRActions()
        {
            
        }
        public Func<NotificationModel, Task> OnAddingPostAction { get; set; }
        public Func<NotificationModel, Task> OnAddingCommentAction { get; set; }
        public Func<Guid , FriendRequestUserAccount, Task> OnSendingFriendRequestAction { get; set; }
        public Func<Guid , UserAccount , Task> OnApprovedFriendRequestAction { get; set; }
        public Func<NotificationModel, Task> OnAddingReactOnPostAction { get; set; }
        public Func<NotificationModel, Task> OnAddingReactOnCommentAction { get; set; }
    }
}
