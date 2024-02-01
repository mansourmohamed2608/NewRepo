using DataBase.Core.Enums;
using DataBase.Core.Models.Accounts;
using DataBase.Core.Models.Notifications;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Posts;
using System.ComponentModel.DataAnnotations;


namespace BDataBase.Core.Models.Accounts
{
    public class UserAccounts
    {
        [Key]
        public Guid Id { get; set; }
        [Key]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Bio { get;set; }
        public string? City { get; set; } 
        public string? Country { get; set; } 
        public DateTime? Birthdate { get; set; }
        public Gender? gender { get; set; }
        public ProfileTypes Type { get; set; } = ProfileTypes.User;
        public CoverPhoto CoverPhoto { get; set; }
        public ProfilePhoto ProfilePhoto { get; set; }
        public ICollection<Post> Posts { get; set; } 
        public ICollection<QuestionPost> QuestionPosts { get; set; } 
        public ICollection<FriendRequest> FriendRequests { get; set; }
        public ICollection<Friend> Friends { get; set; }
        public ICollection<Notifications > Notifications { get; set; }

    }
}
