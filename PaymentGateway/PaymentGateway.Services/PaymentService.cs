using PaymentGateway.Repositories;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Enum;
using PaymentGateway.Repositories.Model;
using PaymentGateway.Services.Validators;

namespace PaymentGateway.Services
{
    public class PaymentService
    {
        private readonly PaymentDbContext _paymentDbContext;
        private readonly CardValidator _cardValidator;

        private readonly AuthenticationService _authenticationService;
        private readonly BankingService _bankingService;

        public PaymentService(PaymentDbContext paymentDbContext,
                              CardValidator cardValidator,
                              AuthenticationService authenticationService,
                              BankingService bankingService)
        {
            _paymentDbContext = paymentDbContext;
            _cardValidator = cardValidator;

            _authenticationService = authenticationService;
            _bankingService = bankingService;
        }

        public MerchantStatus GenerateToken(string clientId, string clientSecret, out string token)
        {
            MerchantStatus merchantStatus = MerchantStatus.NotFound;
            token = string.Empty;

            var merchants = _paymentDbContext.Merchants.Where(p => p.Id.Equals(clientId));
            if (merchants.Any())
            {
                bool isMerchantRegistered = merchants.Any(p => p.Secret.Equals(clientSecret));
                merchantStatus = isMerchantRegistered ? MerchantStatus.Valid : MerchantStatus.PasswordIssue;
                token = merchantStatus != MerchantStatus.Valid ? string.Empty : _authenticationService.GenerateJwtToken(clientId);
            }
            return merchantStatus;
        }

        public ShopperCheckout GetPurchaseDetails(string bankResponseId)
        {
            var paymentDetails = _paymentDbContext.ShopperCheckouts.FirstOrDefault(p => p.BankResponseId.Equals(bankResponseId));
            return paymentDetails;
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request, string merchantId)
        {
            string? bankResponseId = string.Empty;
            _cardValidator.ValidateCardNo(request.CardNumber)
                          .ValidateExpiryDate(request.ExpiryDate)
                          .ValidateCardVerificationValue(request.CVV);

            string status = _cardValidator.ErrorMessages.Any() ? PaymentStatus.Failed.ToString() : PaymentStatus.Started.ToString();
            string messages = _cardValidator.ErrorMessages.Any() ? string.Join(',', _cardValidator.ErrorMessages) : string.Empty;
            var shopper = await InitiateShopperTransactionAsync(request, merchantId, string.Empty, status.ToString(), messages);

            if (!status.Equals("Failed"))
            {
                var responseFromBank = await _bankingService.ProcessPaymentAsync(request);
                bankResponseId = responseFromBank.BankResponseId;
                shopper.BankResponseId = responseFromBank.BankResponseId;
                messages = shopper.TransactionMessages = responseFromBank.Messages;
                status = shopper.PaymentStatus = responseFromBank.Status;

                _paymentDbContext.Update(shopper);
                await _paymentDbContext.SaveChangesAsync();
            }

            PaymentResponse paymentResponse = new PaymentResponse()
            {
                Messages = messages,
                BankResponseId = bankResponseId,
                Status = status
            };

            return paymentResponse;
        }

        private async Task<ShopperCheckout> InitiateShopperTransactionAsync(PaymentRequest? request, string merchantId, string bankResponseId, string status, string messages)
        {
            var addedShopper = await _paymentDbContext.ShopperCheckouts.AddAsync(new ShopperCheckout()
            {
                CheckoutId = request.CheckoutId,
                MerchantId = merchantId,
                BankResponseId = bankResponseId,
                ExpiryDate = request.ExpiryDate,
                Amount = Convert.ToDecimal(request.Amount),
                CreditCardNumber = request.CardNumber.Masked('X', 5, request.CardNumber.Length - 5),
                Currency = request.Currency,
                PaymentStatus = status.ToString(),
                TransactionMessages = messages
            });
            await _paymentDbContext.SaveChangesAsync();
            return addedShopper.Entity;
        }
    }
}
