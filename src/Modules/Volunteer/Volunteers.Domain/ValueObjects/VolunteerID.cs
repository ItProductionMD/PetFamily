namespace Volunteers.Domain.ValueObjects;
public record VolunteerID
{
    public Guid Value { get; }

    protected VolunteerID(Guid id)
    {
        Value = id;
    }

    public static VolunteerID NewGuid() => new(Guid.NewGuid());
    public static VolunteerID Create(Guid id) => new(id);
}

