using GAMAX.Services.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Web;
using DataBase.Core.Models.Authentication;
using DataBase.Core.Enums;
using BDataBase.Core.Models.Accounts;
using DataBase.Core;
using Business.Helper;
using Business;
using Utilites;

namespace GAMAX.Services.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IMailingService _mailingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;


        public AuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt, IMailingService mailingService, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _mailingService = mailingService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return   "Email is already registered!" ;
            var userName = //model.FirstName + Guid.NewGuid().ToString();
            GenerateUserName(model.FirstName, model.LastName);
            while (true)
            {
                if (await _userManager.FindByNameAsync(userName) is not null)
                {
                    userName = GenerateUserName(model.FirstName, model.LastName);
                }
                else
                    break;
            }
            var user = new ApplicationUser
            {

                UserName = userName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Birthdate = model.Birthdate,
                gender = model.gender,
        RegistrationIP = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return  errors ;
            }

            return await SendNewConfirmMail(model.Email);
            
        }
        public async Task<AuthModel> VerifyAsync(VerificationModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthModel { Message = "Email not  registered!" };
            }
            
            var result = await _userManager.ConfirmEmailAsync(user, model.VerificationCode);
            if (!result.Succeeded)
            {
                return new AuthModel { Message = "Wrong  code " };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            await AddProfileAccount(user);
            
            return  new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }
        private async Task AddProfileAccount(ApplicationUser user)
        {
            var isAlreadyUser = _unitOfWork.UserAccounts.Find(i => i.Id.ToString() == user.Id);
            if (isAlreadyUser != null)
                return;
            var profile = new UserAccounts
            {
                Id = Guid.Parse(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName =user.UserName,
                CoverPhoto= new DataBase.Core.Models.PhotoModels.CoverPhoto {Id = Guid.NewGuid(),PhotoPath= AccountHelpers.GetDefaultCoverPohot(Guid.Parse(user.Id)) , UserAccountsId = Guid.Parse(user.Id) },
                ProfilePhoto = new DataBase.Core.Models.PhotoModels.ProfilePhoto { Id = Guid.NewGuid(), PhotoPath = AccountHelpers.GetDefaultProfilePohot(Guid.Parse(user.Id)), UserAccountsId = Guid.Parse(user.Id) } ,
                gender=user.gender,
                Birthdate=user.Birthdate,
                City="",
                Country=""

            };
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                if (Enum.TryParse(roles.First(), out ProfileTypes profileType))
                {
                    profile.Type = profileType;
                }
            }

            // Step 4: Add the new profile to the DbContext and save changes
            await _unitOfWork.UserAccounts.AddAsync(profile);
            await _unitOfWork.Complete();
        }
        public async Task<AuthModel> LoginAndGetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var confirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!confirmed)
                return new AuthModel { Message = "Please Confirm your Email " , IsAuthenticated=false};

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }
        public async Task<AuthModel> GetTokenAsync(string refreshToken)
        {
            var authModel = new AuthModel();

            var user = await FindUserByRefreshToken(refreshToken);

            // Check if the user exists
            if (user is null)
            {
                authModel.Message = "User not found!";
                return authModel;
            }

            // Find the active refresh token
            var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken && t.IsActive);

            if (activeRefreshToken is null)
            {
                authModel.Message = "Refresh token is expired!";
                return authModel;
            }

            // Create a new JWT token
            var jwtSecurityToken = await CreateJwtToken(user);

            // Get the roles assigned to the user
            var rolesList = await _userManager.GetRolesAsync(user);

            // Populate the AuthModel properties
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            authModel.RefreshToken = activeRefreshToken.Token;
            authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;

            return authModel;
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }
        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }
        public async Task<bool> UpdateUserPassword(UpdateUserPassword updateUser)
        {
            var applicationUser = await _userManager.FindByIdAsync(updateUser.Id);
            if (applicationUser == null)
                return false;
            var result = await _userManager.ChangePasswordAsync(applicationUser,updateUser.OldPassword,updateUser.NewPassword);
            if (result.Succeeded)
                return true;
            else
                return false;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("RefreshToken:DurationInDays")),
                CreatedOn = DateTime.UtcNow
            };
        }
        private async Task<ApplicationUser> FindUserByRefreshToken(string refreshToken)
        {

            //TODO wrong logic // to fix search token in token tables then match with user id ;
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));
            return user;
        }

        private string GenerateRandomVerificationCode(int length)
        {
            var random = new Random();
            var code = random.Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString("D" + length);
            return code;
        }
        private string GenerateUserName(string firstName, string lastName)
        {
            firstName = firstName.Trim();
            lastName = lastName.Trim();

            var baseName = $"{firstName}{lastName}";

            var cleanBaseName = new string(baseName.Where(char.IsLetterOrDigit).ToArray());

            var random = new Random();
            var randomNumber = random.Next(10000000, 99999999);

            var userName = $"{cleanBaseName}{randomNumber}";

            return userName;
        }
        public async Task <string> SendNewConfirmMail(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            if (result is null)
                return "this Email is not registered yet!";

            var verificationCode = await _userManager.GenerateEmailConfirmationTokenAsync(result);

            string baseUrl = SharedFolderPaths.BackendUrl;
            string routePrefix = "api/Auth";
            string actionRoute = "verify";

            string url = $"{baseUrl}/{routePrefix}/{actionRoute}?Email={HttpUtility.UrlEncode(result.Email)}&verificationCode={HttpUtility.UrlEncode(verificationCode)}";

            //string url = $"http://localhost:5285/api/Auth/verify?Email={user.Email}&verificationCode={verificationCode}";
            string WelcomeMessage = $@"
                                    <html>
                                        <body>
                                            <h1>Welcome to Gamax!</h1>
                                            <p>Please keep it secret and don't share it with anyone.</p>
                                            <p>
                                                <a href=""{url}"">
                                                    <button style=""background-color: #4CAF50; color: white; padding: 10px 20px; border: none; cursor: pointer;"">
                                                        Click Here
                                                    </button>
                                                </a>
                                            </p>
                                        </body>
                                    </html>";
            await _mailingService.SendEmailAsync(result.Email, "Welcome To Gamax !", WelcomeMessage);
            return  "verification  send To your mail";
        }
        public async Task<string> SendResetPasswordMail(string Email)
        {
            var result = await _userManager.FindByEmailAsync(Email);
            if (result == null)
                return "this Email is not  registered yet!";
            var token = await _userManager.GeneratePasswordResetTokenAsync(result);
            var encrypt = new Secuirty.AES_Security();
            var encryptedEmail= encrypt.Encrypt(result.Email);
            string url = $"http://localhost:3000/resetpassword?t={token}&u={encryptedEmail}";
            string PasswordMail = $@"
                                    <html>
                                        <body>
                                            <h1>Attention!</h1>
                                            <p>Please keep it secret and don't share it with anyone.</p>
                                            <p>
                                                <a href=""{url}"">
                                                    <button style=""background-color: #4CAF50; color: white; padding: 10px 20px; border: none; cursor: pointer;"">
                                                        Click Here
                                                    </button>
                                                </a>
                                            </p>            
                                        </body>
                                    </html>";
            await _mailingService.SendEmailAsync(result.Email, "Gamax Reset Password !", PasswordMail);
            return "reset Password Code Send to your mail";

        }
        public async Task<bool> ResetPassword(RessetPassword model)
        {
            var applicationUser = await _userManager.FindByEmailAsync(model.Email);
            if (applicationUser == null)
                return false;// "this Email is not  registered yet!";
            var result = await _userManager.ResetPasswordAsync(applicationUser, model.Token, model.Password);
            if (result.Succeeded)
              return true;
            else
               return false;
            
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        
    }
}
