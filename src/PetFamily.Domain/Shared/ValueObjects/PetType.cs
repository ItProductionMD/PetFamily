using PetFamily.Domain.DomainResult;
using PetFamily.Domain.PetAggregates.Entities;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record PetType
    {
        public Species Species { get; }
        public Breed Breed { get; }

        private PetType(Breed breed,Species species)
        {
            Species = species;
            Breed = breed;
        }
        public static Result<PetType> Create( Breed? breed,Species? species)
        {
            if (species == null || breed == null)
                return Result<PetType>.Failure("Species and Breed must not be null");
            return Result<PetType>.Success(new PetType(breed,species));
        }
    }
}
