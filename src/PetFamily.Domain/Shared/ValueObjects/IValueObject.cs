namespace PetFamily.Domain.Shared.ValueObjects;

public interface IValueObject
{
    IEnumerable<object> GetEqualityComponents();
}
