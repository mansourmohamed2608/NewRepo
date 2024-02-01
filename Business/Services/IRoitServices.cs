using BDataBase.Core.Models.Accounts;
using DomainModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IRoitServices
    {
        Task<PlayerStatus> GetValorantPlayerStatusAsync(Guid PlayerId);
    }
}
