using CSharpFunctionalExtensions;
using PetFamily.Domain.Aggregates.Species;

namespace PetFamily.Domain.ValueObjects
{
    public class PetType
    {
        public Species Species { get; private set; }
        public Breed Breed { get; private set; }  
        private PetType(Species species, Breed breed)
        {
            Species = species;
            Breed = breed;
        }
        public static Result<PetType> Create(Species? species, Breed? breed)
        {
            if (species == null)
                return Result.Failure<PetType>("Species cannot be null");
            if (breed == null)
                return Result.Failure<PetType>("Breed cannot be null");
            return Result.Success(new PetType(species,breed));
        }
    }
}
