using System.Security.Claims;

namespace GAMAX.Services.Dto
{
    public class UserClaimsInfo
    {
        public string UniqueName { get; set; }
        public string Jti { get; set; }
        public string Email { get; set; }
        public Guid Uid { get; set; }
        public List<Claim> RoleClaims { get; set; }
    }
    public static class UserClaimsHelper
    {
        public static UserClaimsInfo GetClaimsFromHttpContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext context = httpContextAccessor.HttpContext;
            var claimsModel = new UserClaimsInfo
            {
                UniqueName = context.User.FindFirst(ClaimTypes.Name)?.Value,
                Jti = context.User.FindFirst("Jti")?.Value,
                Email = context.User.FindFirst(ClaimTypes.Email)?.Value,
                Uid = Guid.Parse(context.User.FindFirst("uid")?.Value)
            };
            claimsModel.RoleClaims = context.User.Claims
                .Where(c => c.Type.StartsWith(ClaimTypes.Role))
                .ToList();
            return claimsModel;
        }
    }

}
