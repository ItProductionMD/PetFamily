using PetFamily.SharedKernel.Results;
using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteerPhone;
using Volunteers.Public.IContracts;

namespace Volunteers.Application.Contracts;

public class VolunteerPhoneUpdater(
    UpdateVolunteerPhoneHandler handler)
    : IVolunteerPhoneUpdater
{
    public async Task<UnitResult> UpdatePhone(
        Guid userId,
        string phoneRegionCode,
        string phoneNumber,
        CancellationToken ct)
    {
        var command = new UpdateVolunteerPhoneCommand(
            userId,
            phoneRegionCode,
            phoneNumber);

        return await handler.Handle(command, ct);
    }
}
