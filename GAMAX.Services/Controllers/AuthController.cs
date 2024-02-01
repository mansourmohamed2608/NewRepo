using Business;
using DataBase.Core.Models.Authentication;
using GAMAX.Services.Dto;
using GAMAX.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Utilites;

namespace GAMAX.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthService authService , IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if (result != "verification  send To your mail")
                return BadRequest(new { Message = result });
            return Ok(new { Message = result });
        }

        [HttpGet("verify")]
        public async Task<IActionResult> Verify(string Email, string verificationCode)
        {
            VerificationModel model = new VerificationModel();
            model.VerificationCode = verificationCode;
            model.Email = Email;
            if (Email == null && verificationCode == null)
                return BadRequest(new { Message = "wrong params" });

            var result = await _authService.VerifyAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(new { Message = result.Message });

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            string redirectUrl = SharedFolderPaths.orginUrl+"Auth/login?verfiy=true";
            //return Ok();
            return Redirect(redirectUrl);

        }

        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync()
        {
            var refreshTokenFromCookies = Request.Cookies["RefreshToken"];
            if (refreshTokenFromCookies == null)
            {
                return BadRequest(new AuthResponse
                {
                    Message = "Refresh Token Required",
                    IsAuthenticated = false,
                    IsInvaliedAccessToken = true,
                    IsInvalidRefreshToken = true
                });
            }

            var result = await _authService.GetTokenAsync(refreshTokenFromCookies);

            if (!result.IsAuthenticated)
                return Unauthorized(new AuthResponse
                {
                    Message = result.Message,
                    IsAuthenticated = false,
                    IsInvaliedAccessToken = true,
                    IsInvalidRefreshToken = true
                });

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                //SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                SetAccessToken(result.Token,result.ExpiresOn);
            }

            return Ok();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAndGetTokenAsync(model);

            if (!result.IsAuthenticated)
                return Unauthorized(new AuthResponse
                {
                    Message = result.Message,
                    IsAuthenticated = false,
                    IsInvaliedAccessToken = true,
                    IsInvalidRefreshToken = true
                });

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                SetAccessToken(result.Token, result.ExpiresOn);
            }
            //return RedirectToActionPermanent("GetProfileAcountData", "Accounts");
            return Ok();
        }

        [HttpPost("addRole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(new
                {
                    Message = "Failed To Assign The Role"
                });

            return Ok(new
            {
                Message = "Role Added!"
            });
        }

        [HttpPost("revokeTokens")]
        public async Task<IActionResult> RevokeToken()
        {
            var Refreshtoken = Request.Cookies["RefreshToken"] ;

            if (string.IsNullOrEmpty(Refreshtoken))
                return BadRequest(new { Message = "Refresh Token is required!" });

            var result = await _authService.RevokeTokenAsync(Refreshtoken);

            if (!result)
                return BadRequest(new { Message = "Refresh Token is invalid!" });

            var accessToken = Request.Cookies["Authorization"];
            if (accessToken != null)
            {
                RemoveTokenFromCookie("Authorization");
            }

            RemoveTokenFromCookie("RefreshToken");
            return Ok();
        }

        [HttpPost("ResendConfirmMail")]
        public async Task<IActionResult> ResendConfirmMail(string Email)
        {
            var result = await _authService.SendNewConfirmMail(Email);
            if (result != "verification  send to your mail")
                return BadRequest(new
                {
                    Message = result
                });
            return Ok(result);
        }

        [HttpPost("ResetPasswordCode")]
        public async Task<IActionResult> ResetPasswordCode(string Email)
        {
            var result = await _authService.SendResetPasswordMail(Email);
            if (result != "reset Password Code Send to your mail")
                return BadRequest(new
                {
                    Message = result
                });
            return Ok(result);
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] RessetPassword model)
        {
            var security = new Secuirty.AES_Security();
            var Email = security.Decrypt(model.Email);
            model.Email = Email;
            var result = await _authService.ResetPassword(model);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "your data is wrong!"
            });
        }
        [HttpPost("UpdateUserPassword")]
        public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPassword model)
        {
            var userInfo = UserClaimsHelper.GetClaimsFromHttpContext(_httpContextAccessor);
            if (userInfo.Uid.ToString().ToLower() != model.Id.ToLower())
                return Forbid();
            var result = await _authService.UpdateUserPassword(model);
            if (result)
                return Ok(result);
            return BadRequest(new
            {
                Message = "your data is wrong!"
            });
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            RemoveTokenFromCookie("RefreshToken");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }
        private void SetAccessToken(string token , DateTime expires)
        {
            RemoveTokenFromCookie("Authorization");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("Authorization", token, cookieOptions);
        }
        private void RemoveTokenFromCookie(string tokenName)
        {
            //RefreshToken //Authorization
            if (Request.Cookies.TryGetValue(tokenName, out string refreshToken))
            {
                Response.Cookies.Delete(tokenName);
            }
        }

    }
}
