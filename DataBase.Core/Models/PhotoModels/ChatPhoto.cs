using DataBase.Core.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.PhotoModels
{
    public class ChatPhoto:BasePhoto
    {
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
