using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;

namespace PetFamily.Domain.PetManagment.ValueObjects;

public record PetSerialNumber
{
    public int Value { get; }
    private PetSerialNumber(int value)
    {
        Value = value;
    }
    public static Result<PetSerialNumber> Create(int value, Volunteer volunteer)
    {
        var validationResult = Validate(value, volunteer);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new PetSerialNumber(value));
    }
    public static UnitResult Validate(int value, Volunteer volunteer)
    {
        if (value > volunteer.Pets.Count + 1 || value < 0 || value == 0)
        {
            return Result.Fail(Error.InvalidFormat("PetSerialNumber"));
        }
        return UnitResult.Ok();
    }
}
