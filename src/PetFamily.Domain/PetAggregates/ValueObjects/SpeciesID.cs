namespace PetFamily.Domain.PetAggregates.ValueObjects
{
    public record SpeciesID
    {
        public Guid Value { get; }

        private SpeciesID(Guid id)
        {
            Value = id;
        }

        public static SpeciesID NewGuid() => new(Guid.NewGuid());
    }
}
