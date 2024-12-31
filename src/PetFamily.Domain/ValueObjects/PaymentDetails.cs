using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects
{
    public class PaymentDetails
    {
        public string? PaymentName { get; private set; }
        public string? PaymentDescription { get; private set; }
        private PaymentDetails(string? paymentName,string? paymentDescription)
        {
            PaymentName = paymentName;
            PaymentDescription = paymentDescription;
        }
        public static Result<PaymentDetails> Create(string? paymentName, string? paymentDescription)
        {
            //TODO: Add validation
            return Result.Success(new PaymentDetails(paymentName, paymentDescription));
        }
    }
}

