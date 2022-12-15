using System.Net;
using System.Text;
using System.Web;
using Azure.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PaymentGateway.Domain;
using Xunit;

namespace PaymentGateway.Tests.Integration;

public class PaymentControllerTests : IClassFixture<PaymentGatewayWebApplicationFactory<Program>>
{
    private readonly PaymentGatewayWebApplicationFactory<Program> _factory;

    public PaymentControllerTests(PaymentGatewayWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("api/Payment/Token")]
    public async Task GenerateTokenWithValidClientCredentials(string url)
    {
        var client = _factory.CreateClient();
        url = string.Concat(url, "?", GetClientSecretQueryString());

        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.IsNotNull(content);
    }

    [Theory]
    [InlineData("api/Payment/ProcessPayment")]
    public async Task VerifyRequestInvalidToken(string url)
    {
        var client = _factory.CreateClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        requestMessage.Headers.Add("token", "tamperedToken");
        requestMessage.Content = new StringContent(string.Empty, Encoding.UTF8, ContentType.ApplicationJson.ToString());

        HttpResponseMessage paymentResult = await client.SendAsync(requestMessage);
        Assert.AreEqual(HttpStatusCode.InternalServerError, paymentResult.StatusCode);
    }

    [Theory]
    [InlineData("api/Payment/ProcessPayment")]
    public async Task ProcessPayment(string url)
    {
        var client = _factory.CreateClient();
        string tokenUrl = "api/Payment/Token";
        tokenUrl = string.Concat(tokenUrl, "?", GetClientSecretQueryString());

        var tokenResponse = await client.GetAsync(tokenUrl);
        tokenResponse.EnsureSuccessStatusCode();
        var token = await tokenResponse.Content.ReadAsStringAsync();

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        PaymentRequest paymentRequest = new PaymentRequest()
        {
            CheckoutId = "1",
            CardNumber = "378282246310005",
            ExpiryDate = "10/2023",
            CVV = "123",
            Amount = 100.0,
            Currency = "USD"
        };
        requestMessage.Headers.Add("token", token);
        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, ContentType.ApplicationJson.ToString());

        HttpResponseMessage paymentResult = await client.SendAsync(requestMessage);
        paymentResult.EnsureSuccessStatusCode();
        var content = await paymentResult.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<PaymentResponse>(content);
        Assert.AreEqual(response.Status, "Success");
    }

    private string GetClientSecretQueryString()
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["clientId"] = "Amazon_PaymentGateway";
        query["clientSecret"] = "95515b54-8a66-49c7-bdf2-87c38832e736";
        return Convert.ToString(query);
    }
}