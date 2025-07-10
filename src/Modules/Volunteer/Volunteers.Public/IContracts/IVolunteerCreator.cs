using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Public.Dto;

namespace Volunteers.Public.IContracts;

public interface IVolunteerCreator
{

    Task<UnitResult> CreateVolunteer(CreateVolunteerDto createVolunteerDto, CancellationToken ct = default);

}
