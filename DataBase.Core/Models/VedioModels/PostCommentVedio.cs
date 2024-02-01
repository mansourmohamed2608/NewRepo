using DataBase.Core.Models.CommentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.VedioModels
{
    public class PostCommentVedio:BaseVedio
    {
        public Guid PostCommentId { get; set; }
        public PostComment PostComment { get; set; }
    }
}
