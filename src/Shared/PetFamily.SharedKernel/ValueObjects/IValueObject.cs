namespace PetFamily.SharedKernel.ValueObjects;
public interface IValueObject
{
    IEnumerable<object> GetEqualityComponents();
}
