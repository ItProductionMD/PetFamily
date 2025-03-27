namespace PetFamily.Domain.PetManagment.ValueObjects;

public record SpeciesID
{
    public Guid Value { get; }

    private SpeciesID(Guid id)
    {
        Value = id;
    }
    public static SpeciesID NewGuid() => new(Guid.NewGuid());
    public static SpeciesID SetValue(Guid id) => new(id);
}
