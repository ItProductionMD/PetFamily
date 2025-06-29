﻿using PetFamily.SharedKernel.Results;
using System.Text.Json.Serialization;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;


namespace PetFamily.SharedKernel.ValueObjects;

public record RequisitesInfo
{
    public string Name { get; }
    public string Description { get; }

    [JsonConstructorAttribute]
    private RequisitesInfo(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Result<RequisitesInfo> Create(string? name, string? description)
    {
        if (HasOnlyEmptyStrings(name, description))
            return UnitResult.Ok();

        var validationResult = Validate(name, description);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new RequisitesInfo(name!, description!));
    }

    public static UnitResult Validate(string? name, string? description) =>

        UnitResult.FromValidationResults(

            () => ValidateRequiredField(

                valueToValidate: name,
                valueName: "DonateDetails name",
                maxLength: MAX_LENGTH_SHORT_TEXT,
                pattern: null),

            () => ValidateRequiredField(

                valueToValidate: description,
                valueName: "DonateDetails description",
                maxLength: MAX_LENGTH_LONG_TEXT,
                pattern: null));

    //public static UnitResult Validate(DonateDetailsDTO dto) => Validate(dto.Name, dto.Description);


}