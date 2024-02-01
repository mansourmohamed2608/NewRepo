using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.PhotoModels
{
    public class PostCommentPhoto : BasePhoto
    {
        public Guid PostCommentId { get; set; }
        public PostComment PostComment { get; set; }
    }
}
