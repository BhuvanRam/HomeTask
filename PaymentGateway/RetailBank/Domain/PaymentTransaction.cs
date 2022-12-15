using System.ComponentModel.DataAnnotations;

namespace RetailBank.Domain
{
    public record PaymentTransaction
    {
        [Key]
        public Guid PaymentTransactionId { get; set; }
        public string CheckOutId { get; set; }
        public double AmountDeducted { get; set; }
        public string CardNumber { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
