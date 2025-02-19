using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using System.Collections;

namespace PetFamily.Domain.Shared.ValueObjects;


public record ValueObjectList<T> : IReadOnlyList<T>
{
    public IReadOnlyList<T> ValueObjects { get; }

    public int Count => ValueObjects.Count;

    public T this[int index] => ValueObjects[index];

    protected ValueObjectList() { } // EF Core needs this

    public ValueObjectList(IEnumerable<T>? values)
    {
        ValueObjects = values?.ToList() ?? [];
    }

    public static ValueObjectList<TValueObject> Boxing<TDto, TValueObject>(
       IEnumerable<TDto> sourseCollection,
       Func<TDto, Result<TValueObject>> valueObjectFactory)
    {
        var valueObjects = new List<TValueObject>();

        foreach (var dto in sourseCollection)
        {
            var factoryResult = valueObjectFactory(dto);

            if (factoryResult.IsSuccess && factoryResult.Data != null)
            {
                valueObjects.Add(factoryResult.Data);
            }
        }
        var box = new ValueObjectList<TValueObject>(valueObjects);
        return box;
    }

    public static Result<ValueObjectList<TValueObject>> Create<TDto, TValueObject>(
       IEnumerable<TDto> sourseCollection,
       Func<TDto, Result<TValueObject>> valueObjectFactory)
    {
        var valueObjects = new List<TValueObject>();
        List<Error> errors = [];
        foreach (var dto in sourseCollection)
        {
            var factoryResult = valueObjectFactory(dto);

            if (factoryResult.IsSuccess && factoryResult.Data != null)
            {
                valueObjects.Add(factoryResult.Data);
            }
            else
            {
                errors.AddRange(factoryResult.Errors);
            }
        }
        if (errors.Count > 0)
        {
            return Result.Fail(errors);
        }
        var voList = new ValueObjectList<TValueObject>(valueObjects);
        return Result.Ok(voList);
    }

    public static UnitResult Validate<TDto, TValueObject>(
      IEnumerable<TDto> sourseCollection,
      Func<TDto, UnitResult> valueObjectValidation)
    {
        List<Error> errors = [];
        foreach (var dto in sourseCollection)
        {
            var validationResult = valueObjectValidation(dto);

            if (validationResult.IsFailure)
                errors.AddRange(validationResult.Errors);
        }
        if (errors.Count > 0)
            return Result.Fail(errors);

        return UnitResult.Ok();
    }

    public IEnumerator<T> GetEnumerator() => ValueObjects.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ValueObjects.GetEnumerator();
}
