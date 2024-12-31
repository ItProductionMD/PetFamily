using CSharpFunctionalExtensions;
using PetFamily.Domain.ValueObjects;
using PetFamily.Domain.PetEntities.Root;

namespace PetFamily.Domain.VolunteerAggregates.Root
{
    public class Volunteer : Entity<Guid>
    {
        public FullName FullName { get; private set; }
        public string Email { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public int ExpirienceYears { get; private set; }
        public string? Description { get; private set; }
        public DonateDetails? DonateDetails { get; private set; }
        public int CountOfPetsAdopted => Pets.Where(p => p.HelpStatus == HelpStatus.Adopted).Count();
        public int CountOfPetsForAdopt => Pets.Where(p => p.HelpStatus == HelpStatus.ForAdopt).Count();
        public int CountOfPetsForHelp => Pets.Where(p => p.HelpStatus == HelpStatus.ForHelp).Count();
        public List<Pet> Pets { get; private set; } = null!;

        protected Volunteer()
        {
        }
    }
}
