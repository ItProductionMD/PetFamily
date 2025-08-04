using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Application.Validations;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using System.Net.WebSockets;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Domain.ValueObjects;


//-------------------------------Handler,UseCases,Services----------------------------------------//
namespace Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IVolunteerReadRepository volunteerReadRepository,
    IUserContext userContext,
    ILogger<CreateVolunteerHandler> logger) : ICommandHandler<Guid, CreateVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;
    private readonly ILogger<CreateVolunteerHandler> _logger = logger;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result<Guid>> Handle(CreateVolunteerCommand cmd, CancellationToken ct)
    {
        CreateVolunteerValidator.Validate(cmd);

        var adminId = _userContext.GetUserId();

        var volunteer = CreateVolunteerProcess(cmd);

        var addResult = await _volunteerWriteRepository.AddAsync(volunteer, ct);
        if (addResult.IsFailure)
            return addResult;

        _logger.LogInformation("Volunteer with id:{Id},created successful by admin with id:{adminId}!",
            volunteer.Id, adminId);

        return Result.Ok(volunteer.Id);
    }

    private static Volunteer CreateVolunteerProcess(CreateVolunteerCommand cmd)
    {
        var phone = Phone.CreateNotEmpty(cmd.PhoneNumber, cmd.PhoneRegionCode).Data!;

        var fullName = FullName.Create(cmd.FirstName, cmd.LastName).Data!;

        var requisites = cmd.Requisites
            .Select(dto => RequisitesInfo.Create(dto.Name, dto.Description).Data!).ToList();

        return Volunteer.Create(
            VolunteerID.NewGuid(),
            UserId.Create(cmd.UserId).Data!,
            fullName,
            cmd.ExperienceYears,
            cmd.Description,
            phone,
            requisites).Data!;
    }
}
