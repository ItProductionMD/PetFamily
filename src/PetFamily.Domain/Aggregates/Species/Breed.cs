using CSharpFunctionalExtensions;
using PetFamily.Domain.Validation;

namespace PetFamily.Domain.Aggregates.Species
{
    public class Breed: Entity<int>
    {
        public string Name { get; private set; }
        public Species Species { get; private set; }
        protected Breed()
        {

        }
        private Breed(string name,Species species)
        {
            Name = name;
            Species = species;
        }
        public static Result<Breed> Create(string? name,Species? species)
        {
            var validateName= ValidationExtensions.ValidateName(name);
            if(validateName.IsFailure)
                return Result.Failure<Breed>(validateName.Error);
            if (species == null)
                return Result.Failure<Breed>("Species cannot be null");
            return Result.Success(new Breed(name, species));
        }
    }
}
