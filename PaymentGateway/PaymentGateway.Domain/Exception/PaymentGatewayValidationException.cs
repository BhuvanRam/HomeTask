namespace PaymentGateway.Domain.Exception
{
    public class PaymentGatewayValidationException :System.Exception
    {
        public PaymentGatewayValidationException(string message): base(message)
        {
                
        }
    }
}
