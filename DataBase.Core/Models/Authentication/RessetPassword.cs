using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Authentication
{
    public class RessetPassword
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
    public class UpdateUserPassword
    {
        public string Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
