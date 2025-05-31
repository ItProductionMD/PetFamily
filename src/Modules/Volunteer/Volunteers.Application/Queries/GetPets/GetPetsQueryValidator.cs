using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace Volunteers.Application.Queries.GetPets;

public static class GetPetsQueryValidator
{
    public const int MAX_PAGE_SIZE = 1000;
    public const int MIN_AGE_IN_MONTHS = 0;
    public const int MAX_AGE_IN_MONTHS = 500;

    public static UnitResult Validate(GetPetsQuery query)
    {
        var paginationResult = UnitResult.ValidateCollection(
            () => ValidateNumber(query.PageNumber, "pageNumber", 1, int.MaxValue),
            () => ValidateNumber(query.PageSize, "pageSize", 1, MAX_PAGE_SIZE));

        var filterResult = UnitResult.Ok();

        if (query.PetsFilter != null)
        {
            var filter = query.PetsFilter;

            filter.HelpStatuses = filter.HelpStatuses?.Distinct().ToList();
            filter.SpeciesIds = filter.SpeciesIds?.Distinct().ToList();
            filter.BreedIds = filter.BreedIds?.Distinct().ToList();

            filterResult = UnitResult.ValidateCollection(

                () => ValidateNumber(
                    filter.MinAgeInMonth,
                    "Min age in months",
                    MIN_AGE_IN_MONTHS,
                    MAX_AGE_IN_MONTHS),

                () => ValidateNumber(
                    filter.MaxAgeInMonth,
                    "Max age in months",
                    MIN_AGE_IN_MONTHS, MAX_AGE_IN_MONTHS),

                () => filter.MaxAgeInMonth >= filter.MinAgeInMonth
                    ? UnitResult.Ok()
                    : UnitResult.Fail(Error.ValueOutOfRange("MaxAge must be >= MinAge")),

                () => ValidateGuidList(filter.SpeciesIds, "SpeciesIds"),

                () => ValidateGuidList(filter.BreedIds, "BreedIds"),

                () => ValidateNonRequiredField(
                    valueToValidate: filter.City,
                    valueName: "City",
                    maxLength: MAX_LENGTH_SHORT_TEXT,
                    pattern: STREET_PATTERN),

                () => ValidateNonRequiredField(
                     filter.Color,
                     "Color",
                     MAX_LENGTH_SHORT_TEXT,
                     NAME_PATTERN),

                () => ValidateNonRequiredField(
                    filter.PetName,
                    "PetName",
                    MAX_LENGTH_SHORT_TEXT,
                    NAME_PATTERN));
        }

        return UnitResult.ValidateCollection(() => paginationResult, () => filterResult);
    }

    private static UnitResult ValidateGuidList(List<Guid>? list, string listName)
    {
        if (list == null || list.Count == 0)
            return UnitResult.Ok();

        return ValidateItems<Guid>(list, (x) => ValidateIfGuidIsNotEpmty(x, listName));
    }
}
