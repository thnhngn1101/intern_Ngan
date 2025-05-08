using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common.Application.Exceptions;
using Common.Application.Models;
using Common.Application.Settings;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;

namespace Common.Utilities
{
    public static class JwtUtil
    {
        public static string CreateJwtToken(JwtTokenSetting jwtTokenSetting, AuthenticatedUserModel user, string userRole)
        {
            var expires = DateTime.Now.AddDays(jwtTokenSetting.ExpirationDays);
            var signingCredentials = GetSignatureKey(jwtTokenSetting.SymmetricSecurityKey);

            var tokenClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, jwtTokenSetting.JwtRegisteredClaimNamesSub),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                new(JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddDays(jwtTokenSetting.ExpirationDays).ToUnixTimeSeconds().ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Role, userRole)
            };

            tokenClaims.AddRange(UserClaims.GetTokenClaims(user));

            var token = new JwtSecurityToken(
                jwtTokenSetting.Issuer,
                jwtTokenSetting.Audience,
                tokenClaims,
                expires: expires,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public static AuthenticatedUserModel VerifyAndGetUserModelFromJwtToken(string jwtToken, JwtTokenSetting jwtTokenSetting)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSetting.SymmetricSecurityKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenSetting.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtTokenSetting.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out _);
                var result = UserClaims.GetAuthenticatedUser(principal.Claims);

                return result;
            }
            catch (Exception ex)
            {
                throw new NonAuthenticateException(ex.Message);
            }
        }

        private static SigningCredentials GetSignatureKey(string serectKey)
        {
            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(serectKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }

        public static class UserClaims
        {
            public const string UserId = "user.id";
            public const string UserName = "user.username";
            public const string FirstName = "user.firstname";
            public const string LastName = "user.lastname";
            public const string Email = "user.email";

            public static List<Claim> GetTokenClaims(AuthenticatedUserModel user)
            {
                return new List<Claim>
                {
                    new(UserId, user.UserId.ToString()),
                    new(UserName, user.UserName),
                    new(FirstName, user.FirstName ?? string.Empty),
                    new(LastName, user.LastName ?? string.Empty),
                    new(ClaimTypes.Role, user.Role) 
                };
            }
            public static AuthenticatedUserModel GetAuthenticatedUser(IEnumerable<Claim> claims)
            {
                var claimDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var roles = new List<string>();

                foreach (var claim in claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        roles.Add(claim.Value);
                    }
                    else
                    {
                        if (!claimDictionary.ContainsKey(claim.Type))
                        {
                            claimDictionary[claim.Type] = claim.Value;
                        }
                    }
                }

                string role = roles.Any() ? string.Join(",", roles) : "Guest";

                if (!claimDictionary.ContainsKey(UserClaims.UserId) || !claimDictionary.ContainsKey(UserClaims.UserName))
                {
                    throw new InvalidOperationException("Missing required claims: UserId or UserName");
                }

                return new AuthenticatedUserModel
                {
                    UserId = new Guid(claimDictionary[UserClaims.UserId]),
                    UserName = claimDictionary[UserClaims.UserName],
                    FirstName = claimDictionary.ContainsKey(UserClaims.FirstName) ? claimDictionary[UserClaims.FirstName] : string.Empty,
                    LastName = claimDictionary.ContainsKey(UserClaims.LastName) ? claimDictionary[UserClaims.LastName] : string.Empty,
                    Role = role 
                };
            }

        }
    }

}
