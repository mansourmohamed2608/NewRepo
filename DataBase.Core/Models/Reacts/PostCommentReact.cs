using DataBase.Core.Models.CommentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Reacts
{
    public class PostCommentReact: BaseReact
    {
        public Guid PostCommentId { get; set; }
        public PostComment PostComment { get; set; }
    }
}
