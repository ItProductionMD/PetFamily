using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;

namespace PetFamily.Domain.Shared.ValueObjects;

public record PetType
{
    public Guid SpeciesId { get; }
    public Guid BreedId { get; }

    private PetType() { }//EF core need this

    private PetType(Guid breedId, Guid speciesId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public static Result<PetType> Create(BreedID breedId, SpeciesID speciesId)
    {
        var validationResult = Validate(breedId, speciesId);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new PetType(breedId.Value, speciesId.Value));
    }

    public static UnitResult Validate(BreedID breedId, SpeciesID speciesId)
    {
        if (breedId.Value == Guid.Empty)
            return UnitResult.Fail(Error.GuidIsEmpty("PetType breedId"));

        if (speciesId.Value == Guid.Empty)
            return UnitResult.Fail(Error.GuidIsEmpty("PetType speciesId"));

        return UnitResult.Ok();
    }
}