using CSharpFunctionalExtensions;

namespace PetFamily.Domain.PetEntities.PetType
{
    public class Breed : Entity<int>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public int SpeciesId { get; private set; }

        protected Breed()
        {
        }

        private Breed(string? name, string? description, int speciesId)
        {
            Name = name ?? "Other";
            Description = description;
            SpeciesId = speciesId;
        }
    }
}
