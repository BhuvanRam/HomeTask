using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaymentGateway.Domain;

namespace PaymentGateway.Services
{
    public class AuthenticationService
    {
        private readonly JwtConfiguration _configuration;
        public AuthenticationService(IOptions<JwtConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public string GenerateJwtToken(string clientId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.Key);
            int slidingExpirationInMinutes = Convert.ToInt32(_configuration.SlidingExpirationInMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", clientId) }),
                Expires = DateTime.UtcNow.AddMinutes(slidingExpirationInMinutes),
                Issuer = _configuration.Issuer,
                Audience = _configuration.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
