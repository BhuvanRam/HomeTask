namespace PaymentGateway.Domain
{
    public class JwtConfiguration
    {
        public string Key { get; set; }
        public int SlidingExpirationInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
