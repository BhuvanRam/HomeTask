using Microsoft.EntityFrameworkCore;
using RetailBank.Domain;

namespace RetailBank.Repositories
{
    public class RetailBankDBContext : DbContext
    {
        public RetailBankDBContext(DbContextOptions<RetailBankDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopperDetail>()
                .HasData
                (
                    new ShopperDetail(){ CardNumber = "378282246310005", ExpiryDate = "10/2023", CVV = "123", Balance = 200.00 ,Currency = "USD"},
                    new ShopperDetail() { CardNumber = "371449635398431", ExpiryDate = "10/2024", CVV = "456", Balance = 200.00, Currency = "EUR" },
                    new ShopperDetail() { CardNumber = "378734493671000", ExpiryDate = "10/2025", CVV = "789", Balance = 200.00, Currency = "INR" }
                );
        }

        public DbSet<ShopperDetail> ShopperDetails { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
