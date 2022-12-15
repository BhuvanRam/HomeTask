namespace RetailBank.Domain.Constants
{
    public static class PaymentConstants
    {
        public const string Failed = "Payment Failed. Please retry after sometime";
        public const string Success = "Payment Successfull";
        public const string InProgress =
            "Payment is in Progress. Relay Url is sent back in URL to check the status after some time";
        public const string InvalidCardDetails = "Payment Failed. Card details are invalid";
        public const string InsufficientBalance = "Payment Failed. Balance is insufficient";
    }
}
