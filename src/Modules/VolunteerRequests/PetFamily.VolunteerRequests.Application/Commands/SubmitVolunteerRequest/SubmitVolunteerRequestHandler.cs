using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Application.IRepositories;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;

public class SubmitVolunteerRequestHandler(
    IUserContext userContext,
    IVolunteerRequestWriteRepository requestWriteRepository,
    IVolunteerRequestReadRepository requestReadRepository,
    ILogger<SubmitVolunteerRequestHandler> logger) : ICommandHandler<SubmitVolunteerRequestCommand>
{
    private readonly IVolunteerRequestWriteRepository _requestWriteRepository = requestWriteRepository;
    private readonly IUserContext _userContext = userContext;
    private readonly IVolunteerRequestReadRepository _requestReadRepository = requestReadRepository;
    private readonly ILogger<SubmitVolunteerRequestHandler> _logger = logger;

    public async Task<UnitResult> Handle(SubmitVolunteerRequestCommand cmd, CancellationToken ct)
    {
        var validationResult = SubmitVolunteerRequestValidator.Validate(cmd);
        if (validationResult.IsFailure)
        {
            var errorMessage = validationResult.ToErrorMessage();
            _logger.LogWarning("Create volunteer request failed! Error:{message}", errorMessage);

            return UnitResult.Fail(validationResult.Error);
        }

        var getUserId = _userContext.GetUserId();
        if (getUserId.IsFailure)
        {
            _logger.LogWarning("UserId in userContext not found !Error:{error}",
                getUserId.ToErrorMessage());
            return UnitResult.Fail(getUserId.Error);
        }
        var userId = getUserId.Data!;

        var isRequestExist = await _requestReadRepository.CheckIfRequestExistAsync(userId, ct);
        if (isRequestExist)
        {
            _logger.LogWarning("Volunteer request already exists for user {UserId}", userId);
            return UnitResult.Fail(Error.ValueIsAlreadyExist("Volunteer request already exists"));
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

        await _requestWriteRepository.AddAsync(volunteerRequest, ct);

        await _requestWriteRepository.SaveAsync(ct);

        _logger.LogInformation("Volunteer request submitted successfully for user {UserId}", userId);

        return await Task.FromResult<UnitResult>(UnitResult.Ok());
    }
}
