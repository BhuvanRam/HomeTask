namespace PaymentGateway.Domain.Constants
{
    public static class MerchantStatusMessages
    {
        public const string BadRequest = "Invalid Merchant Status. Contact Payment Gateway Administrator";
        public const string PasswordIssue = "Merchant client secret is invalid";
        public const string NotFound = "Merchant not registered in Payment Gateway";
    }
}
