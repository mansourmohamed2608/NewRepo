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
    public class QuestionComment:BaseComment
    {
        public Guid QuestionPostId { get; set; }
        public QuestionPost QuestionPost { get; set; }
        public QuestionCommentPhoto QuestionCommentPhoto { get; set; }
        public QuestionCommentVedio QuestionCommentVedio { get; set; }
        public List<QuestionCommentReact> QuestionCommentReacts { get; set; }
    }
}
