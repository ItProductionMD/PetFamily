using PetFamily.Domain.Shared;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.Results;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Domain.PetManagment.Entities;

public class Breed : Entity<Guid>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private Breed(Guid id) : base(id) { }//Ef core needs this

    private Breed(Guid id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public static Result<Breed> Create(BreedID id, string? name, string? description)
    {
        var validationResult = Validate(name, description);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Breed(id.Value, name!, description));
    }

    public static UnitResult Validate(string? name, string? description) =>

        UnitResult.ValidateCollection(

            () => ValidateRequiredField(name, "Breed name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateNonRequiredField(description, "Breed description", MAX_LENGTH_LONG_TEXT));

}
