namespace PaymentGateway.Domain
{
    public class PaymentResponse
    {
        public string Status { get; set; }
        public string BankResponseId { get; set; }
        public string Messages { get; set; }
    }
}
