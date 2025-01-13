using PetFamily.Domain.PetAggregates.Entities;

namespace PetFamily.Domain.PetAggregates.ValueObjects;
public record BreedID 
{
    public Guid Value;
    private BreedID(Guid id)
    {
        Value = id;
    }
    public static BreedID NewGuid()=> new(Guid.NewGuid());

}
