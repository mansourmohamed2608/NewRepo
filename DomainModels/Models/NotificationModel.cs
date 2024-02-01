using DataBase.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Models
{
    public class NotificationModel
    {
        public Guid Id { get; set; }
        public Guid ActionedUserId { get; set; }
        public Guid NotifiedUserId { get; set; }
        public Guid ItemId { get; set; }
        public NotificatinTypes NotificatinType { get; set; }

    }
}
