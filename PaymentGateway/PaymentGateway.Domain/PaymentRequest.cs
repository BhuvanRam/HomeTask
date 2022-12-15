using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Domain
{
    public record PaymentRequest
    {
        [Required]
        public string CheckoutId { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryDate { get; set; }
        [Required]
        [Range(0, 1000)]
        public double Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string CVV { get; set; }
    }
}
