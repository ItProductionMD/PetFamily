namespace PetFamily.Domain.VolunteerAggregates.ValueObjects;
public record VolunteerID 
{
    public Guid Value { get; }
    protected VolunteerID(Guid id) 
    {
        Value = id;
    }
    public static VolunteerID NewGuid()=>new(Guid.NewGuid());
}

