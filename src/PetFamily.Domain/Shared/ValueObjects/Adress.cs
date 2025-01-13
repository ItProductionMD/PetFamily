using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Domain.Shared.ValueObjects;

public record Adress
{
    public string Street { get; }
    public string City { get; }
    public string Region { get; }
    public string Number { get; }
    private const string REGION = "Adress region";
    private const string CITY = "Adress city";
    private const string STREET = "Adress street";
    private const string HOME_NUMBER = "Adress number";
    private Adress() { }//EF core need this
    private Adress(string region,string city,string street,string number)
    {
        Region = region;
        City = city;
        Street = street;
        Number = number;
    }
    public static Result<Adress?> Create(string? country,string? city,string? street,string? number,bool IsRequired)
    {       
        var hasOnlyEmptyStrings = HasOnlyEmptyStrings(country,city,street,number);
        if (hasOnlyEmptyStrings && !IsRequired)
            return Result<Adress?>.Success(null);
        var validationResult = Validate(country,city,street,number);
        if (validationResult.IsFailure)
            return Result<Adress?>.Failure(validationResult.Error!);
        return Result<Adress?>.Success(new Adress(country!, city!, street!, number!));
    }      
    private static Result Validate(string? region, string? city, string? street, string? number)=>
        ValidateRequiredField(region, REGION, MAX_LENGTH_SHORT_TEXT, NAME_PATTERN)
        .OnFailure(() => ValidateRequiredField(city, CITY, MAX_LENGTH_SHORT_TEXT, NAME_PATTERN))
        .OnFailure(() => ValidateRequiredField(street, STREET, MAX_LENGTH_SHORT_TEXT, STREET_PATTERN))
        .OnFailure(() => ValidateRequiredField(number, HOME_NUMBER, MAX_LENGTH_SHORT_TEXT, ADRESS_NUMBER_PATTERN));       
}
