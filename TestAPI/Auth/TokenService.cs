using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestAPI.Auth
{
    public class TokenService : ITokenService
    {
        private TimeSpan ExpiryDuration = new TimeSpan(0, 30, 0);
        public string BuildToken(string key, string issuer, APIUserDto user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };

            var securityKey = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.Add(ExpiryDuration), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
