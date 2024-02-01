using BDataBase.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Accounts
{
    public class Friend
    {
        public Guid Id { get; set; }
        public Guid FirstUserId { get; set; }
        public UserAccounts FirstUser { get; set; }
        public Guid SecondUserId { get; set; }
        public UserAccounts SecondUser { get; set; }
        public DateTime ApprovedTime { get; set; } = DateTime.UtcNow;
    }
}
