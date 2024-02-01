using Business.Implementation;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValorantController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoitServices _roitServices;
        public ValorantController(IRoitServices roitServices , IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _roitServices = roitServices;
        }
        [HttpPost("GetPlayerValorantData")]
        public async Task<IActionResult> GetPlayerValorantData(Guid playerId)
        {
            var data = await _roitServices.GetValorantPlayerStatusAsync(playerId);
            return Ok(data);
        }
    }
}
