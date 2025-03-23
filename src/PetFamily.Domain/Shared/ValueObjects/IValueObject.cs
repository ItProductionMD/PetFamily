using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PetFamily.Domain.Shared.ValueObjects;

public interface IValueObject
{
    IEnumerable<object> GetEqualityComponents();
}
