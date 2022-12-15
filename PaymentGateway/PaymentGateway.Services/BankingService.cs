using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentGateway.Domain;

namespace PaymentGateway.Services
{
    public class BankingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public BankingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public virtual async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest? paymentRequest)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, _configuration["RetailBankUrl"])
            {
                Content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage paymentResult = await _httpClient.SendAsync(requestMessage);
            paymentResult.EnsureSuccessStatusCode();

            string result = await paymentResult.Content.ReadAsStringAsync();
            var jResult = JObject.Parse(result);
            
            PaymentResponse response = new PaymentResponse()
            {
                BankResponseId = jResult["paymentTransactionId"]?.Value<string>(),
                Status = jResult["status"]?.Value<string>(),
                Messages = jResult["message"]?.Value<string>()
            };
            return response;
        }
    }
}
