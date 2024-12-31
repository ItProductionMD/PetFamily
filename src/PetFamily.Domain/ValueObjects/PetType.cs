using CSharpFunctionalExtensions;

namespace PetFamily.Domain.ValueObjects
{
    public class PetType: ValueObject
    {
        public int SpeciesId { get; private set; }
        public int BreedId { get;private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return SpeciesId;
            yield return BreedId;
        }
    }
}
