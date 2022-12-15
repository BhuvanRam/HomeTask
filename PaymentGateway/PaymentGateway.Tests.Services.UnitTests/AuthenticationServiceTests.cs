using Microsoft.Extensions.Options;
using PaymentGateway.Domain;
using PaymentGateway.Services;

namespace PaymentGateway.Tests.Services.UnitTests
{
    [TestClass]
    public class AuthenticationServiceTests
    {
        private AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            IOptions<JwtConfiguration> options = Options.Create(new JwtConfiguration()
            {
                Key = "SecretKey_13_12_2022_Hyderabad",
                Issuer = "https://localhost:7274",
                SlidingExpirationInMinutes = 30
            });

            _authenticationService = new AuthenticationService(options);
        }

        [TestMethod]
        public void GenerateJWtToken_Success()
        {
            string token = _authenticationService.GenerateJwtToken("ApplePay");
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public void GenerateJWtToken_ThrowException()
        {
            IOptions<JwtConfiguration> options = Options.Create(new JwtConfiguration());
            _authenticationService = new AuthenticationService(options);

            Assert.ThrowsException<ArgumentNullException>(() => _authenticationService.GenerateJwtToken("ApplePay"));
        }
    }
}
