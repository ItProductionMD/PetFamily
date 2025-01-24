using PetFamily.Domain.Shared.DomainResult;

namespace PetFamily.Domain.Shared.ValueObjects;

public static class ValueObjectFactory
{

    /// <summary>
    /// Converts a collection of DTOs into a list of value objects using the provided factory function.
    /// </summary>
    /// <typeparam name="TDto">The type of the DTO objects in the source collection.</typeparam>
    /// <typeparam name="TValueObject">The type of the value objects to create.</typeparam>
    /// <param name="sourceDtos">The collection of DTOs to process.</param>
    /// <param name="valueObjectFactory">
    /// A factory function that takes a DTO as input and returns a <see cref="Result{TValueObject}"/> containing the value object.
    /// </param>
    /// <returns>A list of successfully created value objects.</returns>
    public static IReadOnlyList<TValueObject> MapDtosToValueObjects<TDto, TValueObject>(
        IEnumerable<TDto> sourceDtos,
        Func<TDto, Result<TValueObject>> valueObjectFactory)
    {
        var valueObjects = new List<TValueObject>();

        foreach (var dto in sourceDtos)
        {
            var factoryResult = valueObjectFactory(dto);

            if (factoryResult.IsSuccess && factoryResult.Data != null)
            {
                valueObjects.Add(factoryResult.Data);
            }
        }
        return valueObjects;
    }
}
