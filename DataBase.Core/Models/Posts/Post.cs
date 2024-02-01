using BDataBase.Core.Models.Accounts;
using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Posts
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        public string ? Title { get; set; }
        public string? Description { get; set; }
        public DateTime TimeCreated { get; set; }  = DateTime.UtcNow;
        public ICollection<PostPhoto> Photos { get; set; }
        public ICollection<PostVedio> Vedios { get; set; }
        public ICollection<PostComment> Comments { get; set; }
        public ICollection<PostReact> Reacts { get; set; }

        //Forign-key
        public UserAccounts UserAccounts { get; set; }
        public Guid UserAccountsId { get; set; }
    }
}
