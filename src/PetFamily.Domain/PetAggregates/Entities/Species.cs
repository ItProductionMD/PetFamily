using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.PetAggregates.ValueObjects;

namespace PetFamily.Domain.PetAggregates.Entities
{
    public class Species : Entity<Guid>
    {
        public string Name { get; private set; }

        private readonly List<Breed> _breeds = [];
        public IReadOnlyList<Breed> Breeds => _breeds;

        private Species(Guid id) : base(id) { }//Ef core needs this

        private Species(Guid id, string name) : base(id)
        {
            Name = name;
        }

        public void AddBreed(Breed breed) => _breeds.Add(breed);

        public int GetBreedCount() => _breeds.Count;

        public static Result<Species> Create(SpeciesID id, string? name)
        {
            var validationResult = Validate(name);
            if (validationResult.IsFailure)
                return Result<Species>.Failure(validationResult.Error!);

            return Result<Species>.Success(new Species(id.Value, name!));
        }

        public static Result Validate(string? name) =>

            ValidateRequiredField(name,"Species name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN);
    }
}