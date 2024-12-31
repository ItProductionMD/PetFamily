using CSharpFunctionalExtensions;


namespace PetFamily.Domain.ValueObjects
{
    public class Adress:ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string Number { get; private set; }
        protected Adress()
        {
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return Country;
            yield return Number;
        }
    }
}
