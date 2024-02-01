using DataBase.Core.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Reacts
{
    public class PostReact: BaseReact
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}
