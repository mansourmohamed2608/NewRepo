using BDataBase.Core.Models.Accounts;
using System.ComponentModel.DataAnnotations;

namespace DataBase.Core.Models.CommentModels
{
    public class BaseComment
    {
        [Key]
        public Guid Id { get; set; }
        public string? comment { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public UserAccounts UserAccounts { get; set; }
        public Guid UserAccountsId { get; set; }
    }
}
