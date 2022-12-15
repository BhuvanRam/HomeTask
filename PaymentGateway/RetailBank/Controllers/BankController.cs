using Microsoft.AspNetCore.Mvc;
using RetailBank.Repositories;
using RetailBank.Domain;
using RetailBank.Domain.Constants;
using RetailBank.Domain.Enum;

namespace RetailBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly RetailBankDBContext _retailBankDbContext;
        public BankController(RetailBankDBContext retailBankDbContext)
        {
            _retailBankDbContext = retailBankDbContext;
        }

        [HttpPost]
        [Route("ProcessPayment")]
        public IActionResult ProcessPayment(PaymentRequest bankRequest)
        {
            var cardDetails = _retailBankDbContext.ShopperDetails.FirstOrDefault
                                                                (p =>
                                                                    p.CardNumber.Equals(bankRequest.CardNumber) &&
                                                                    p.Currency.Equals(bankRequest.Currency) &&
                                                                    p.CVV.Equals(bankRequest.CVV) &&
                                                                    p.ExpiryDate.Equals(bankRequest.ExpiryDate)
                                                                );

            PaymentStatus status = PaymentStatus.InvalidCardDetails;
            string message = PaymentConstants.InvalidCardDetails;
            if (cardDetails != null)
            {
                bool isBalanceAvailable = cardDetails.Balance - bankRequest.Amount >= 0;
                status = isBalanceAvailable ? PaymentStatus.Success : PaymentStatus.InsufficientBalance;
                message = isBalanceAvailable ? PaymentConstants.Success : PaymentConstants.InsufficientBalance;

                if (status == PaymentStatus.Success)
                {
                    cardDetails.Balance = cardDetails.Balance - bankRequest.Amount;
                    _retailBankDbContext.ShopperDetails.Update(cardDetails);
                }
            }

            Guid bankResponseId = Guid.NewGuid();
            PaymentTransaction transaction = new PaymentTransaction()
            {
                CheckOutId = bankRequest.CheckoutId,
                PaymentTransactionId = bankResponseId,
                Status = status.ToString(),
                Message = message
            };
            _retailBankDbContext.PaymentTransactions.Add(transaction);
            _retailBankDbContext.SaveChanges();

            return Ok(transaction);
        }

    }
}
