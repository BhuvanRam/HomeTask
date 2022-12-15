using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PaymentGateway.Domain;
using PaymentGateway.Services;

namespace PaymentGateway.Tests.Services.UnitTests
{
    [TestClass]
    public class BankingServiceTests
    {
        private BankingService _bankingService;

        [TestMethod]
        public void ProcessPayment_Success()
        {
            // Arrange
            var paymentConfigurations = new Dictionary<string, string> { {"RetailBankUrl", "http://www.somerichbank.com/api/processpayment"}};
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(paymentConfigurations).Build();
            PaymentRequest paymentRequest = new PaymentRequest()
            {
                CheckoutId = "1",
                CardNumber = "378282246310005",
                ExpiryDate = "10/2023",
                CVV = "123",
                Amount = 100.0,
                Currency = "USD"
            };

            PaymentTransaction paymentResponse = new PaymentTransaction()
            {
                paymentTransactionId = Guid.NewGuid(),
                status = "Success",
                message = "Payment Successfull",
            };

            HttpResponseMessage responseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(paymentResponse), Encoding.UTF8, "application/json")
            };

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            
            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _bankingService = new BankingService(httpClient, configuration);

            // Act
            var response = _bankingService.ProcessPaymentAsync(paymentRequest).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(paymentResponse.paymentTransactionId.ToString(), response.BankResponseId);
            Assert.AreEqual(paymentResponse.message, response.Messages);
            Assert.AreEqual(paymentResponse.status, response.Status);
        }
    }
    
    public record PaymentTransaction
    {
        public Guid paymentTransactionId { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}
