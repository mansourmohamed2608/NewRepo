using DataBase.Core.Models.CommentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Reacts
{
    public class QuestionCommentReact: BaseReact
    {
        public Guid QuestionCommentId { get; set; }
        public QuestionComment QuestionComment { get; set; }


    }
}
