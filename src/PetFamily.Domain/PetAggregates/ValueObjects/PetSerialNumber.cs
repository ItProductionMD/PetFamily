using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Domain.PetAggregates.ValueObjects;

public record PetSerialNumber
{
    public int Value { get; }
    private PetSerialNumber(int value)
    {
        Value = value;
    }
    public static Result<PetSerialNumber> Create(int value , Volunteer volunteer)
    {
        var validationResult = Validate(value,volunteer);
        if (validationResult.IsFailure)
            return Result<PetSerialNumber>.Failure(validationResult.Errors!);

        return Result<PetSerialNumber>.Success(new(value));
    }
    public static Result Validate(int value, Volunteer volunteer)
    {
        if (value > volunteer.Pets.Count || value < 0)
        {
            return Result.Failure(Error.CreateErrorInvalidFormat("PetSerialNumber"));
        }
        return Result.Success();
    }
}
