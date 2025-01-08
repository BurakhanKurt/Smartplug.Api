using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Smartplug.Application.Settings;
using Smartplug.Domain.Entities;
using Smartplug.Domain.Models;
using Smartplug.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Smartplug.Api.Application.Jwt
{
    public class JwtGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<JwtGenerator> _logger;
        private readonly SmartplugDbContext _DbContext;

        public JwtGenerator(IOptions<JwtSettings> jwtSettings, UserManager<Users> userManager, ILogger<JwtGenerator> logger, RoleManager<Role> roleManager, SmartplugDbContext ykDbContext, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
            _roleManager = roleManager;
            _DbContext = ykDbContext;
        }

        public async Task<ValidateTokenResult> ValidateToken(string token, TokenType tokenType = TokenType.AccessToken)
        {
            if (token == null)
            {
                //_logger.SendError($"Validate line 96| Token is null | TokenType => {tokenType}", nameof(ValidateToken));
                return new ValidateTokenResult(false, "Please provide valid token!");
            }


            //string userid = string.Empty;

            //if (userid.IsNullOrEmpty())
            //{
            //    return new ValidateTokenResult(false, "Token not found!");
            //}

            //var userGuid = Guid.Parse(userid);
            //var existUser = await _userManager.Users.AnyAsync(x => x.Id == userGuid);

            //if (!existUser)
            //{
            //    //_logger.SendError($"Validate line 130 | user null | TokenType => {tokenType}", nameof(ValidateToken));
            //    return new ValidateTokenResult(false, "Token not found!");
            //}

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,

                }, out SecurityToken validatedToken);

                return new ValidateTokenResult(true, string.Empty, GetClaim(token, "userid"));
            }
            catch (SecurityTokenExpiredException ex)
            {
                //_logger.SendError(ex, $"Validate line 176 | token not valid | TokenType => {tokenType}", nameof(ValidateToken));
                return new ValidateTokenResult(false, "Token has expired! Please login to get new token!");
            }
            catch (Exception e)
            {
                //_logger.LogError($"Validate line 181 | {nameof(JwtGenerator)} throw an exception. Exception: {e.Message}", e);
                return new ValidateTokenResult(false, e.Message);
            }
        }
        public async Task<double?> GetExpireTime(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var expiryTimeUnix = jsonToken?.ValidTo;
                    if (!expiryTimeUnix.HasValue)
                        return null;
                    return (expiryTimeUnix.Value - DateTime.UtcNow).TotalSeconds;
                }
                else
                {
                    // Token parse edilemedi veya JWT değil.
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GenerateJwt)} throw an exception. Exception: {ex.Message}", ex);
                return null;
            }
        }
        public async Task<string> GenerateJwt(Users user)
        {
            string token = "";
            try
            {
                var listOfRoles = await _userManager.GetRolesAsync(user);
                string rolesTxt = "Public";

                if (listOfRoles.Count > 0)
                    rolesTxt = listOfRoles.Aggregate((a, b) => a + "," + b);

                var claims = new List<Claim>();

                claims.AddRange(await _userManager.GetClaimsAsync(user));

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(await _roleManager.Roles.FirstOrDefaultAsync(w => w.Name.Equals(role)));
                    if (roleClaims != null)
                    {
                        foreach (var roleClaim in roleClaims)
                        {
                            if (!claims.Any(c => c.Type == roleClaim.Type && c.Value == roleClaim.Value))
                            {
                                claims.Add(roleClaim);
                            }
                        }
                    }
                }

                claims.AddRange(new List<Claim>()
                {
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("role", rolesTxt)
                });


                claims.Add(new Claim("userid", user.Id.ToString()));


                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.UtcNow.AddHours(_jwtSettings.Ttl);

                var rawToken = new JwtSecurityToken(
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    claims.Distinct(),
                    expires: expires,
                    signingCredentials: signIn);

                token = new JwtSecurityTokenHandler().WriteToken(rawToken);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GenerateJwt)} throw an exception. Exception: {ex.Message}", ex);
            }


            return token;
        }
        public async Task<string> GenerateRefreshToken(Users user)
        {
            string token = "";
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var rawToken = new JwtSecurityToken(
                    null,
                    null,
                    expires: DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTtl),
                    signingCredentials: signIn,
                    claims: new Claim[] { new Claim("userid", user.Id.ToString()) });
                token = new JwtSecurityTokenHandler().WriteToken(rawToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GenerateRefreshToken)} throw an exception. Exception: {ex.Message}", ex);
            }

            return token;

        }
        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(token))
            {
                //_logger.SendError($"Validate | Token can not read", nameof(GetClaim));
                return string.Empty;
            }

            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
            return stringClaimValue;
        }
        public async Task Logout(Users user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "AccessToken");
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
        }
        public enum TokenType
        {
            RefreshToken, AccessToken
        };

    }
}
