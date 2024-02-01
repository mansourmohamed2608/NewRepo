using BDataBase.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Accounts
{
    public class FriendRequest
    {
        public Guid Id { get; set; }
        public Guid RequestorId { get; set; }
        public UserAccounts Requestor { get; set; }

        // User who received the friend request
        public Guid ReceiverId { get; set; }
        public UserAccounts Receiver { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public bool Seen { get; set; }

    }
}
