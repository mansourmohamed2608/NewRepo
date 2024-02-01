using BDataBase.Core.Models.Accounts;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Posts
{
    public class QuestionPost
    {
        [Key]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
        public ICollection<QuestionPhoto> Photos { get; set; }
        public ICollection<QuestionVedio> Vedios { get; set; }
        public ICollection<QuestionComment> Comments { get; set; }
        public ICollection<QuestionReact> Reacts { get; set; }

        //Forign-key
        public UserAccounts UserAccounts { get; set; }
        public Guid UserAccountsId { get; set; }
    }
}
