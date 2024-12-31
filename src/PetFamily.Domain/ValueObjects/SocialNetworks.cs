using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects
{
    public class SocialNetworks : ValueObject
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        private SocialNetworks()
        {
           
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Url;
        }
    }
}
