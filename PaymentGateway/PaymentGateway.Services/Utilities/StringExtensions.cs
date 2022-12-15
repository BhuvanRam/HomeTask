namespace PaymentGateway.Services
{
    public static class StringExtensions
    {
        public static string Masked(this string source, char mask, int start, int count)
        {
            var firstPart = source.Substring(0, start);
            var lastPart = source.Substring(start + count);
            var middlePart = new string(mask, count);
            return firstPart + middlePart + lastPart;
        }
    }
}
