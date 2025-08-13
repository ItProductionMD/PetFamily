using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteerPhone;

public class UpdateVolunteerPhoneHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ILogger<UpdateVolunteerPhoneHandler> logger) : ICommandHandler<UpdateVolunteerPhoneCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepo = volunteerWriteRepo;
    private readonly ILogger<UpdateVolunteerPhoneHandler> _logger = logger;
    public async Task<UnitResult> Handle(UpdateVolunteerPhoneCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var getVolunteer = await _volunteerWriteRepo.GetByUserIdAsync(cmd.UserId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;
     
        var phoneResult = Phone.CreateNotEmpty(cmd.PhoneNumber, cmd.PhoneRegionCode);
        if (phoneResult.IsFailure)
            return UnitResult.Fail(phoneResult.Error);

        volunteer.UpdatePhone(phoneResult.Data!);

        return await _volunteerWriteRepo.SaveAsync(volunteer, ct);
    }
}
