using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using PaymentGateway.Domain;
using PaymentGateway.Repositories;
using PaymentGateway.Services;

namespace PaymentGateway.Tests.Integration;

public class PaymentGatewayWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PaymentDbContext>));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            services.AddDbContext<PaymentDbContext>(options =>
            {
                options.UseInMemoryDatabase("PaymentGatewayIntegration");
            });
        });

        builder.ConfigureTestServices(services =>
        {
            BankingService bankingService = MockBankingService();
            services.AddSingleton(bankingService);
        });

        builder.UseEnvironment("Development");
    }

    private BankingService MockBankingService()
    {
        Mock<HttpClient> httpClientMock = new Mock<HttpClient>();
        Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();
        Mock<BankingService> bankingService = new Mock<BankingService>(MockBehavior.Loose,  httpClientMock.Object, configurationMock.Object );

        var request = new PaymentRequest()
        {
            CheckoutId = "1",
            CardNumber = "378282246310005",
            ExpiryDate = "10/2023",
            CVV = "123",
            Amount = 100.0,
            Currency = "USD"
        };
        var response = new PaymentResponse()
        {
            BankResponseId = Guid.NewGuid().ToString(),
            Messages = "Payment Successfull",
            Status = "Success"
        };
        bankingService.Setup(p => p.ProcessPaymentAsync(request)).ReturnsAsync(response);

        return bankingService.Object;
    }
}