
namespace PetFamily.Domain.Shared.ValueObjects;


public record ValueObjectList<T> where T : class
{
    public IReadOnlyList<T> ObjectList { get; }

    protected ValueObjectList() { }//Ef core need this

    public ValueObjectList(IReadOnlyList<T>? values)
    {
        ObjectList = values ?? [];
    }

}
