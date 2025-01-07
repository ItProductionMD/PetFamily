using PetFamily.Domain.DomainResult;
using PetFamily.Domain.PetAggregates.Entities;

namespace PetFamily.Domain.Shared.ValueObjects
{
    public record PetType
    {
        public Guid SpeciesId { get; }
        public Guid BreedId { get; }

        private PetType() { }//EF core need this
        private PetType(Guid breedId,Guid speciesId)
        {
            SpeciesId = speciesId;
            BreedId = breedId;
        }
        public static Result<PetType> Create(Guid breedId,Guid speciesId)
        {
            //TODO add validations
            return Result<PetType>.Success(new PetType(breedId,speciesId));
        }
    }
}
