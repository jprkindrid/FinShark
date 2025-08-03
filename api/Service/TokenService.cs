using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;
        private readonly SymmetricSecurityKey key;
        private readonly ILogger<TokenService> logger;

        public TokenService(IConfiguration config, ILogger<TokenService> logger)
        {
            this.config = config;
            this.logger = logger;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RequiredConfig.Get(config, "JWT:SigningKey")));
        }

        public string CreateToken(AppUser user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Email, user.Email!),
                    new(JwtRegisteredClaimNames.GivenName, user.UserName!)
                };

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = creds,
                    Issuer = RequiredConfig.Get(config, "JWT:Issuer"),
                    Audience = RequiredConfig.Get(config, "JWT:Audience")
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                logger.LogInformation("JWT token created for user: {Username}", user.UserName);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create JWT token for user: {Username}", user.UserName);
                throw;
            }
        }
    }
}
