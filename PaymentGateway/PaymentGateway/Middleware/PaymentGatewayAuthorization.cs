using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PaymentGateway.Middleware
{
    public class PaymentGatewayAuthorization
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public PaymentGatewayAuthorization(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string token = httpContext.Request.Headers["token"];
            ValidateToken(httpContext, token);
            return _next(httpContext);
        }

        private void ValidateToken(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
                ValidateAudience = false,
                ValidateIssuer = false
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var clientId = jwtToken.Claims.FirstOrDefault(x => x.Type == "id").Value;
            context.User.Identities.FirstOrDefault()?.AddClaim(new Claim("client id", clientId));
        }
    }
}
