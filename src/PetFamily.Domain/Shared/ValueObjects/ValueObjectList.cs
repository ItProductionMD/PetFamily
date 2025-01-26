
namespace PetFamily.Domain.Shared.ValueObjects;


public record ValueObjectList<T> where T : class
{
    public IReadOnlyList<T> ObjectList { get; }

    protected ValueObjectList() { } // EF Core needs this

    public ValueObjectList(IEnumerable<T>? values)
    {
        ObjectList = values?.ToList() ?? [];
    }
}
