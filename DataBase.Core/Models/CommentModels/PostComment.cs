using DataBase.Core.Models.PhotoModels;
using DataBase.Core.Models.Posts;
using DataBase.Core.Models.Reacts;
using DataBase.Core.Models.VedioModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.CommentModels
{
    public class PostComment:BaseComment
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public PostCommentPhoto PostCommentPhoto { get; set; }
        public PostCommentVedio PostCommentVedio { get; set; }
        public List<PostCommentReact> PostCommentReacts { get; set; }
    }
}
