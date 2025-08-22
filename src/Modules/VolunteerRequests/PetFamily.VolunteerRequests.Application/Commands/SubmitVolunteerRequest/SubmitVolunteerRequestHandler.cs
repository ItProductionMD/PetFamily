using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;

public class SubmitVolunteerRequestHandler(
    IVolunteerRequestWriteRepository requestWriteRepo,
    IVolunteerRequestReadRepository requestReadRepo,
    ILogger<SubmitVolunteerRequestHandler> _logger) : ICommandHandler<SubmitVolunteerRequestCommand>
{
    public async Task<UnitResult> Handle(SubmitVolunteerRequestCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var userId = cmd.UserId;

        var isRequestExist = await requestReadRepo.CheckIfRequestExistAsync(userId, ct);
        if (isRequestExist)
        {
            _logger.LogWarning("Volunteer request already exists for user {UserId}", userId);
            return UnitResult.Fail(Error.Conflict("Volunteer request already exists"));
        }

        var requisites = cmd.Requisites.Select(r => RequisitesInfo.Create(r.Name, r.Description).Data!);

        var volunteerRequest = VolunteerRequest.Create(
            userId,
            cmd.DocumentName,
            cmd.LastName,
            cmd.FirstName,
            cmd.Description,
            cmd.ExperienceYears,
            requisites).Data!;

        await requestWriteRepo.AddAsync(volunteerRequest, ct);


        await requestWriteRepo.SaveAsync(ct);

        _logger.LogInformation("Volunteer request submitted successfully for user {UserId}", userId);

        return UnitResult.Ok();
    }
}
