using DataBase.Core.Models.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace GAMAX.Services.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<AuthModel> VerifyAsync(VerificationModel model);
        Task<AuthModel> LoginAndGetTokenAsync(TokenRequestModel model);
        Task<AuthModel> GetTokenAsync(string refreshToken);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<string> SendNewConfirmMail(string email);
        Task<string> SendResetPasswordMail(string email);
        Task<bool> ResetPassword(RessetPassword user);
        Task<bool> UpdateUserPassword(UpdateUserPassword updateUser);
    }
    
}
