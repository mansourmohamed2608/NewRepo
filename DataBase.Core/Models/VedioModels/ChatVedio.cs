using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.VedioModels
{
    public class ChatVedio:BaseVedio
    {
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
