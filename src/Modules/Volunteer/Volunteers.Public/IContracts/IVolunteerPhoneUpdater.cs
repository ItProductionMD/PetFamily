using PetFamily.SharedKernel.Results;

namespace Volunteers.Public.IContracts;

public interface IVolunteerPhoneUpdater
{
    Task<UnitResult> UpdatePhone(
        Guid userId,
        string phoneRegionCode,
        string phoneNumber,
        CancellationToken ct);
}
