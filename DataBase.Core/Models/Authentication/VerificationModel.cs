using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Authentication
{
    public class VerificationModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
