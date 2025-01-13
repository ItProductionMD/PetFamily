using PetFamily.Domain.Shared.DomainResult;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;

namespace PetFamily.Domain.Shared.ValueObjects;
public record Phone
{
    public string Number { get; }
    public string RegionCode { get; }
    private const string NUMBER = "Phone number";
    private const string REGION_CODE = "Phone region code";
    private Phone(string number, string regionCode)
    {
        Number = number;
        RegionCode = regionCode;
    }
    public static Result<Phone> Create(string? number, string? regionCode)
    {
        var validationResult = Validate(number, regionCode);
        if (validationResult.IsFailure)
            return Result<Phone>.Failure(validationResult.Error!);
        return Result<Phone>.Success(new Phone(number!, regionCode!));
    }
    private static Result Validate(string? number, string? regionCode) =>
        ValidateRequiredField(number, NUMBER, MAX_LENGTH_SHORT_TEXT, PHONE_NUMBER_PATTERN)
        .OnFailure(()=>ValidateRequiredField(regionCode,REGION_CODE,MAX_LENGTH_SHORT_TEXT,PHONE_REGION_PATTERN));       
}