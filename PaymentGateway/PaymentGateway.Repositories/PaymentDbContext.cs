using Microsoft.EntityFrameworkCore;
using PaymentGateway.Repositories.Model;

namespace PaymentGateway.Repositories
{
    public sealed class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Merchant>()
                        .HasData
                        (
                            new Merchant() { Id = "Apple_PaymentGateway", Secret = "e3dfb1ad-6178-4637-960f-cd10401be136" },
                            new Merchant() { Id = "Amazon_PaymentGateway", Secret = "95515b54-8a66-49c7-bdf2-87c38832e736" }
                        );
        }

        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<ShopperCheckout> ShopperCheckouts { get; set; }
    }
}
