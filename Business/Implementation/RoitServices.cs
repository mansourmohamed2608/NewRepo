using Business.Services;
using DomainModels.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Business.Implementation
{
    public class RoitServices : IRoitServices
    {
        public async Task<PlayerStatus> GetValorantPlayerStatusAsync(Guid PlayerId)
        {
            var playerData = await Utilites. Services.CallApiUrlAsync("www.roit.developers.api/");
            return new PlayerStatus
            {
                Data = playerData,
            };
        }
    }
}
