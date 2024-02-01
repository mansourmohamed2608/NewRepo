using DataBase.Core.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.PhotoModels
{
    public class QuestionPhoto : BasePhoto
    {
        public Guid QuestionId { get; set; }
        public QuestionPost QuestionPost { get; set; }
    }
}
