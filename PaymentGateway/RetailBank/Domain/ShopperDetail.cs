using System.ComponentModel.DataAnnotations;

namespace RetailBank.Domain
{
    public record ShopperDetail
    {
        [Key]
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public string CVV { get; set; }
    }
}
