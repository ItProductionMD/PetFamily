using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Shared.DomainResult;

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
        var validatePetType = Validate(breedId, speciesId);

        if (validatePetType.IsFailure)
            return Result<PetType>.Failure(validatePetType.Errors!);

        return Result<PetType>.Success(new PetType(breedId.Value, speciesId.Value));
    }

    private static Result Validate(BreedID breedId, SpeciesID speciesId)
    {
        if (breedId.Value == Guid.Empty)
            return Result.Failure(Error.CreateErrorGuidIdIsEmpty("PetType breedId"));

        if (speciesId.Value == Guid.Empty)
            return Result.Failure(Error.CreateErrorGuidIdIsEmpty("PetType speciesId"));

        return Result.Success();
    }
}