using PetFamily.Domain.Shared;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.Results;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Domain.PetManagment.Entities
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
                return validationResult;

            return Result.Ok(new Species(id.Value, name!));
        }
        public static UnitResult Validate(string? name) =>

            ValidateRequiredField(name, "Species name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN);

        public void AddBreeds(List<Breed> breeds)
        {
            _breeds.AddRange(breeds);
        }

    }
}