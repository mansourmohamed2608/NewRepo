using Business;
using GAMAX.Services.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Utilites;

namespace GAMAX.Services.MiddleWare
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string accessToken = context.Request.Cookies["Authorization"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = GetTokenValidationParameters();
                    ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
                    context.User= claimsPrincipal;
                    var roles = claimsPrincipal.FindAll("roles").Select(c => c.Value).ToList();
                    context.Items["Roles"] = roles;
                }
                catch (SecurityTokenException)
                {
                    var responseMessage = new  AuthResponse { 
                        Message = "Access Token is invalid or Expired" ,
                        IsAuthenticated = false ,
                        IsInvaliedAccessToken = true ,
                        IsInvalidRefreshToken=false
                    };
                    var responseJson = JsonSerializer.Serialize(responseMessage);
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseJson, Encoding.UTF8);
                    return;
                }
            }
            else
            {
                // Access token is missing
                var responseMessage = new { Message = "Access Token is Missing!" };
                var responseJson = JsonSerializer.Serialize(responseMessage);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(responseJson, Encoding.UTF8);
                return;
            }

            // Call the next middleware or controller
            await _next(context);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();

            // Read the values from the configuration file
            string validIssuer = SharedFolderPaths.validIssuer;
            string validAudience = SharedFolderPaths.validAudience;
            string key = SharedFolderPaths.key;
            // Create and configure the token validation parameters
            var validationParameters = new TokenValidationParameters
            {
                // Set your token validation settings here (e.g., issuer, audience, signing key)
                ValidIssuer = validIssuer,
                ValidAudience = validAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            return validationParameters;
        }
    }


}
