using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Authentication
{
    public class TokenCode
    {
        [Key]
        public string Token { get; set; }
        public string Code { get; set; }
    }
}
