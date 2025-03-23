namespace PetFamily.Domain.PetManagment.ValueObjects;

public record PetID
{
    public Guid Value { get; }

    private PetID(Guid id)
    {
        Value = id;
    }

    public static PetID NewGuid() => new(Guid.NewGuid());

}
