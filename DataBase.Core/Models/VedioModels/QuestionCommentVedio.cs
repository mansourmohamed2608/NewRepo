using DataBase.Core.Models.CommentModels;
using DataBase.Core.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.VedioModels
{
    public class QuestionCommentVedio : BaseVedio
    {
        public Guid QuestionCommentId { get; set; }
        public QuestionComment QuestionComment { get; set; }
    }
}
