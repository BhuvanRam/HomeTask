namespace RetailBank.Domain
{
    public record PaymentRequest
    {
        public string CheckoutId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string CVV { get; set; }
        
    }
}
