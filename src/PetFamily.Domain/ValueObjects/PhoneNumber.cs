using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Number { get; private set; }
        public string RegionCode { get; private set; }
        protected PhoneNumber()
        {
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return RegionCode;
        }
    }
}
