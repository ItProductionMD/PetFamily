using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using System.Security.AccessControl;
using PetFamily.Domain.Results;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Shared.Validations;
using PetFamily.Application.Abstractions;


//-------------------------------Handler,UseCases,Services----------------------------------------//
namespace PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    IVolunteerReadRepository volunteerReadRepository,
    IValidator<CreateVolunteerCommand> validator,
    ILogger<CreateVolunteerHandler> logger):ICommandHandler<Guid,CreateVolunteerCommand>
{
    private readonly IVolunteerWriteRepository _volunteerWriteRepository = volunteerWriteRepository;
    private readonly IVolunteerReadRepository _volunteerReadRepository = volunteerReadRepository;
    private readonly IValidator<CreateVolunteerCommand> _validator = validator;
    private readonly ILogger<CreateVolunteerHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(CreateVolunteerCommand command, CancellationToken cancelToken)
    {
        var fluentValidationResult = await _validator.ValidateAsync(command, cancelToken);
        if (fluentValidationResult.IsValid == false)
        {
            var errorResult = fluentValidationResult.ToResultFailure<Guid>();
            _logger.LogError("Fail validate volunteer command!{Errors}",
                errorResult.ValidationMessagesToString());

            return errorResult;
        }

        var checkUniqueness = await _volunteerReadRepository.CheckUniqueFields(
            Guid.Empty,
            command.PhoneRegionCode,
            command.PhoneNumber,
            command.Email,
            cancelToken);
        if(checkUniqueness.IsFailure)
            return checkUniqueness;

        var volunteer = CreateVolunteerProccess(command);

        var addResult = await _volunteerWriteRepository.AddAsync(volunteer, cancelToken);
        if (addResult.IsFailure)
            return addResult;

        _logger.LogInformation("Volunteer with id:{Id},created sucessfull!", volunteer.Id);

        return addResult;
    }

    private static Volunteer CreateVolunteerProccess(CreateVolunteerCommand command)
    {
        var fullName = FullName.Create(command.FirstName, command.LastName).Data!;

        var phone = Phone.CreateNotEmpty(command.PhoneNumber, command.PhoneRegionCode).Data!;

        var socialNetworkList = command.SocialNetworksList
            .Select(dto => SocialNetworkInfo.Create(dto.Name, dto.Url).Data!).ToList();

        var requisites = command.Requisites
            .Select(dto => RequisitesInfo.Create(dto.Name, dto.Description).Data!).ToList();

        return Volunteer.Create(
            VolunteerID.NewGuid(),
            fullName,
            command.Email,
            phone,
            command.ExperienceYears,
            command.Description,
            requisites,
            socialNetworkList).Data!;
    }
}
