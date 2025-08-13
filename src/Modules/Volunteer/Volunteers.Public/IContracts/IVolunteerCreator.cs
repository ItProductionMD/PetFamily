using PetFamily.SharedKernel.Results;
using Volunteers.Public.Dto;

namespace Volunteers.Public.IContracts;

public interface IVolunteerCreator
{

    Task<Result<Guid>> CreateVolunteer(CreateVolunteerDto createVolunteerDto, CancellationToken ct = default);

}
