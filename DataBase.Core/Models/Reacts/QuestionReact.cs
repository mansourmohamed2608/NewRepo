using DataBase.Core.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Reacts
{
    public class QuestionReact : BaseReact
    {
        public Guid QuestionPostId { get; set; }
        public QuestionPost QuestionPost { get; set; }
    }
}
