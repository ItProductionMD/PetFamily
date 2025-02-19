using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;

namespace PetFamily.Application.Pets;

public interface IPetRepository
{
    Task<UnitResult> AddAsync(Volunteer volunteer, Pet newPet);
    Task<Result<Pet>> GetAsync(Guid petId,CancellationToken cancellationToken);
    Task<UnitResult> UpdateImages(Pet pet,CancellationToken cancellationToken);
}
