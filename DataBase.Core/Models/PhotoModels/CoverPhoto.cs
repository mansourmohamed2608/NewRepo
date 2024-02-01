using BDataBase.Core.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Core.Models.PhotoModels
{
    public class CoverPhoto:BasePhoto
    {
        public Guid UserAccountsId { get; set; }
    }
}
