using PaymentGateway.Services.Validators;
using Xunit;

namespace PaymentGateway.Tests.Services.UnitTests
{
    public class CardValidatorTests
    {
        private readonly CardValidator _cardValidator;

        public CardValidatorTests()
        {
            _cardValidator = new CardValidator();
        }
    
        [Theory]
        [InlineData("378282246310005")]
        [InlineData("371449635398431")]
        [InlineData("378734493671000")]
        public void CardValidation_Success(string cardNumber)
        {
            _cardValidator.ValidateCardNo(cardNumber);
            Assert.IsTrue(!_cardValidator.ErrorMessages.Any());
        }

        [Theory]
        [InlineData("371449635393281")]
        [InlineData("378734493670320")]
        public void CardValidation_Failure(string cardNumber)
        {
            _cardValidator.ValidateCardNo(cardNumber);
            Assert.IsTrue(_cardValidator.ErrorMessages.Any());
        }

        [Theory]
        [InlineData("01/2027")] 
        [InlineData("05/2027")]
        [InlineData("06/2027")]
        public void ExpiryDateValidation_Success(string expiryDate)
        {
            _cardValidator.ValidateExpiryDate(expiryDate);
            Assert.IsTrue(!_cardValidator.ErrorMessages.Any());
        }

        [Theory]
        [InlineData("01/2030")]
        [InlineData("05/2030")]
        [InlineData("06/2030")]
        public void ExpiryDateValidation_Failure(string expiryDate)
        {
            _cardValidator.ValidateExpiryDate(expiryDate);
            Assert.IsFalse(!_cardValidator.ErrorMessages.Any());
        }

        [Theory]
        [InlineData("209")]
        [InlineData("721")]
        [InlineData("127")]
        public void CardVerificationValue_Success(string cvv)
        {
            _cardValidator.ValidateCardVerificationValue(cvv);
            Assert.IsTrue(!_cardValidator.ErrorMessages.Any());
        }

        [Theory]
        [InlineData("20c")]
        [InlineData("72b")]
        [InlineData("12a")]
        [InlineData("1234")]
        public void CardVerificationValue_Failure(string cvv)
        {
            _cardValidator.ValidateCardVerificationValue(cvv);
            Assert.IsFalse(!_cardValidator.ErrorMessages.Any());
        }
    }
}
