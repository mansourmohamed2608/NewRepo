using DataBase.Core.Enums;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace DomainModels.DTO
{
    #region Posts
    public record UpdateQuestion
    {
        public Guid Id { get; set; }
        public List<Guid>? DeletedPhotoIds { get; set; }
        public List<Guid>? DeletedVedioIds { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public PostsTypes Type { get; set; } = PostsTypes.Question;
        public string Question { get; set; }
        public string? Answer { get; set; }
    }
    public record UpdatePost
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public List<Guid>? DeletedPhotoIds { get; set; }
        public List<Guid>? DeletedVedioIds { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public PostsTypes Type { get; set; } = PostsTypes.Post;
    }
    public record UploadPost
    {
        public string Description { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public PostsTypes Type { get; set; } = PostsTypes.Post;
    }
    public record UploadQuestionPost
    {
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public PostsTypes Type { get; set; } = PostsTypes.Question;
        public string Question { get; set; }
        public string? Answer { get; set; }
    }
    public record QuestionPostDTO
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string? Answer { get; set; }
        public string TimeCreated { get; set; }
        public ICollection<BasePhoto> Photos { get; set; }
        public ICollection<BaseVedio> Vedios { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
        public ICollection<ReactsDTO> Reacts { get; set; }
        public int commentsCount { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime Time { get; set; }
    }
    public record PostDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string TimeCreated { get; set; }
        public ICollection<BasePhoto> Photos { get; set; }
        public ICollection<BaseVedio> Vedios { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
        public ICollection<ReactsDTO> Reacts { get; set; }
        public int commentsCount { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime Time { get; set; }

    }
    public record AllPostDTO
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string TimeCreated { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public PostsTypes Type { get; set; }
        public ICollection<BasePhoto> Photos { get; set; }
        public ICollection<BaseVedio> Vedios { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
        public ICollection<ReactsDTO> Reacts { get; set; }
        public int commentsCount { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime Time { get; set; }
    }

    #endregion

    #region comments
    public record CommentUpdateRequest
    {
        public Guid Id { get; set; }
        public string? comment { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Vedio { get; set; }
        public Guid? DeletedPhotoId { get; set; }
        public Guid? DeletedVedioId { get; set; }

    }
    public record AddCommentRequest
    {
        public string? comment { get; set; }
        public Guid PostId { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Vedio { get; set; }

    }
    public record CommentDTO
    {
        public Guid Id { get; set; }
        public string? comment { get; set; }
        public string Date { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public DateTime Time { get; set; }
        public Guid UserId { get; set; }
        public BasePhoto? CommentPhoto { get; set; }
        public BaseVedio? CommentVedio { get; set; }
        public List<ReactsDTO> CommentReacts { get; set; }

    }
    public record CommentUpdate
    {
        public Guid Id { get; set; }
        public string comment { get; set; }
        public Guid PostId { get; set; }
        public PostsTypes PostsType { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Vedio { get; set; }
    }

    #endregion

    #region React
    public record ReactsDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public ReactsType react { get; set; }

    }
    #endregion

    #region UserProfile
    public record UserAccount
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? gender { get; set; }
        public string Type { get; set; }
    }
    public record FriendRequestUserAccount
    {
        public Guid RequestId { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? gender { get; set; }
        public string Type { get; set; }
    }
    public record ProfileUpdateModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? gender { get; set; } = "Unknown";
    }

    #endregion

    #region Notification
    public record NotificationDTO
    {
        public Guid Id { get; set; }
        public Guid NotifiedUserId { get; set; }
        public Guid ActionedUserId { get; set; }
        public string ActionUserFirstName { get; set; }
        public string ActionUserLastName { get; set; }
        public Guid ItemId { get; set; }
        public Guid PostId { get; set; }
        public PostsTypes PostsType { get; set; }
        public NotificatinTypes NotificatinType { get; set; }
    }
    public record NotificationModel
    {
        public Guid Id { get; set; }
        public Guid NotifiedUserId { get; set; }
        public Guid ActionedUserId { get; set; }
        public string ActionUserFirstName { get; set; }
        public string ActionUserLastName { get; set; }
        public Guid PostId { get; set; }
        public PostsTypes PostsType { get; set; }
        public NotificatinTypes NotificatinType { get; set; }
        public string TimeCreated { get; set; }
    }
    #endregion

    #region chat
    public record ChatDTO
    {
        public Guid? Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReciveId { get; set; }
        public string? Message { get; set; }
        public ICollection<BasePhoto>? Photos { get; set; }
        public ICollection<BaseVedio>? Vedios { get; set; }
        public bool Read { get; set; }
        public string TimeCreated { get; set; }
        public DateTime TimeStamp { get; set; }
    }
    public record UploadChatDTO
    {
        public Guid? Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReciveId { get; set; }
        public string? Message { get; set; }
        //public List<Guid>? DeletedPhotoIds { get; set; }
        //public List<Guid>? DeletedVedioIds { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public bool Read { get; set; } = false;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
    public record UpdateChatDTO
    {
        public Guid? Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReciveId { get; set; }
        public string? Message { get; set; }
        public List<Guid>? DeletedPhotoIds { get; set; }
        public List<Guid>? DeletedVedioIds { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<IFormFile>? Vedios { get; set; }
        public bool Read { get; set; } = false;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
    #endregion

    public record PlayerStatus
    {
        public string Data { get; set; }
    }
}
