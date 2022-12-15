
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Constants;
using PaymentGateway.Domain.Enum;
using PaymentGateway.Domain.Exception;
using PaymentGateway.Services;

namespace PaymentGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpGet]
        [Route("Token")]
        public IActionResult Token(string clientId, string clientSecret)
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new PaymentGatewayValidationException(
                    "clientId or clientSecret cannot be empty in Action Token");

            _logger.LogInformation($"Begin Token Action. ClientId is {clientId} and ClientSecret is {clientSecret}");
            MerchantStatus merchantStatus = _paymentService.GenerateToken(clientId, clientSecret, out string token);
            IActionResult response = BadRequest(MerchantStatusMessages.BadRequest);
            switch (merchantStatus)
            {
                case MerchantStatus.PasswordIssue: response = Unauthorized(MerchantStatusMessages.PasswordIssue);
                    break;
                case MerchantStatus.NotFound: response = NotFound(MerchantStatusMessages.NotFound);
                    break;
                case MerchantStatus.Valid: response = Ok(token);
                    break;
            }
            _logger.LogInformation($"End Token Action");
            return response;
        }

        [HttpPost]
        [Route("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment(PaymentRequest? paymentRequest)
        {
            if(!ModelState.IsValid)
                throw new PaymentGatewayValidationException(
                    $"PaymentRequest is invalid in Action ProcessPayment:  {JsonConvert.SerializeObject(paymentRequest)}");

            _logger.LogInformation($"Begin ProcessPayment. PaymentRequest Payload: {JsonConvert.SerializeObject(paymentRequest)}");
            var merchantId = HttpContext.User.Claims.FirstOrDefault(p => p.Type == "client id")?.Value;
            var paymentResponseObject = await _paymentService.ProcessPaymentAsync(paymentRequest, merchantId);
            _logger.LogInformation($"End ProcessPayment.");
            return Ok(paymentResponseObject);
        }

        [HttpGet]
        [Route("RetrievePaymentDetails")]
        public IActionResult RetrievePaymentDetails(string bankResponseId)
        {
            if (string.IsNullOrEmpty(bankResponseId))
                throw new PaymentGatewayValidationException(
                    "BankResponseId cannot be empty in Action RetrievePaymentDetails");

            _logger.LogInformation($"Begin RetrievePaymentDetails. BankResponseId: {bankResponseId}");
            var paymentDetails = _paymentService.GetPurchaseDetails(bankResponseId);
            _logger.LogInformation($"End RetrievePaymentDetails.");
            return Ok(paymentDetails);
        }
    }
}
