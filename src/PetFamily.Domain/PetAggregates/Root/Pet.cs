using CSharpFunctionalExtensions;
using PetFamily.Domain.ValueObjects;

namespace PetFamily.Domain.PetEntities.Root
{
    public class Pet : Entity<Guid>
    {
        public string Name { get; private set; }
        public DateOnly? DateOfBirth { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public string? Description { get; private set; }
        public bool? IsVaccinated { get; private set; }
        public bool? IsSterilized { get; private set; }
        public double? Weight { get; private set; }
        public double? Height { get; private set; }
        public string? Color { get; private set; }
        public ValueObjects.PetType PetType { get; private set; } //Dog, Cat, Bird, etc
        public PhoneNumber? OwnerPhone { get; private set; }
        public DonateDetails? DonateDetails { get; private set; }
        public string? HealthInfo { get; private set; }
        public Adress? Adress { get; private set; }
        public HelpStatus HelpStatus { get; private set; }

        protected Pet()
        {
            DateTimeCreated = DateTime.UtcNow;
        }
    }
    public enum HelpStatus
    {
        Adopted,
        ForAdopt,
        ForHelp,
    }
}
