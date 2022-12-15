using System.Text.RegularExpressions;
using PaymentGateway.Domain.Constants;

namespace PaymentGateway.Services.Validators
{
    public class CardValidator
    {
        private readonly List<string> _errorMessages;
        public IEnumerable<string> ErrorMessages => _errorMessages;

        public CardValidator()
        {
            _errorMessages = new List<string>();
        }

        public CardValidator ValidateCardVerificationValue(string cvv)
        {
            var cvvCheck = new Regex(@"^\d{3}$");
            if (!cvvCheck.IsMatch(cvv))
                _errorMessages.Add(CardValidationErrorMessages.InvalidCardVerificationValue);
            return this;
        }

        public CardValidator ValidateExpiryDate(string expiryDate)
        {
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            bool isResult = true;

            var dateParts = expiryDate.Split('/');
            isResult = !(!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1]));

            if (isResult)
            {
                var year = int.Parse(dateParts[1]);
                var month = int.Parse(dateParts[0]);
                var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month);
                var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);


                isResult = (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
            }
            if(!isResult)
                _errorMessages.Add(CardValidationErrorMessages.InvalidExpiryDate);

            return this;
        }

        public CardValidator ValidateCardNo(string cardNo)
        {
            cardNo = cardNo.Replace("-", "");
            int cardLength = cardNo.Length, nSum = 0;
            bool isResult = false;

            if (!string.IsNullOrEmpty(cardNo))
            {
                bool isSecond = false;
                for (int counter = cardLength - 1; counter >= 0; counter--)
                {
                    int digit = cardNo[counter] - '0';
                    if (isSecond)
                        digit = digit * 2;

                    nSum += digit / 10;
                    nSum += digit % 10;
                    isSecond = !isSecond;
                }
                isResult = nSum % 10 == 0;
            }

            if (!isResult)
                _errorMessages.Add(CardValidationErrorMessages.InvalidCardNumber);

            return this;
        }
    }
}
