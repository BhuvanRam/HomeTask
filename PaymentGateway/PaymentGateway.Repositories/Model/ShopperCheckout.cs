using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Repositories.Model
{
    public record ShopperCheckout
    {
        [Key]
        public string CheckoutId  { get; set; }
        public string MerchantId { get; set; }
        public string CreditCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public string? BankResponseId { get; set; }
        public string TransactionMessages { get; set; }
        public virtual Merchant Merchant { get; set; }
    }
}
