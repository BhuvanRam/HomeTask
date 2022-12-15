using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Repositories.Model
{
    public record Merchant
    {
        [Key]
        public string Id { get; set; }
        public string Secret { get; set; }
    }
}
