using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteerPhone;

public class UpdateVolunteerPhoneHandler(IVolunteerWriteRepository volunteerWriteRepo)
    : ICommandHandler<UpdateVolunteerPhoneCommand>
{
    public async Task<UnitResult> Handle(UpdateVolunteerPhoneCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var getVolunteer = await volunteerWriteRepo.GetByUserIdAsync(cmd.UserId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var phoneResult = Phone.CreateNotEmpty(cmd.PhoneNumber, cmd.PhoneRegionCode);
        if (phoneResult.IsFailure)
            return UnitResult.Fail(phoneResult.Error);

        volunteer.UpdatePhone(phoneResult.Data!);

        return await volunteerWriteRepo.SaveAsync(volunteer, ct);
    }
}
