using DataBase.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }
       public DateTime Birthdate { get; set; }
       public  Gender gender { get; set; }
        public string RegistrationIP { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
