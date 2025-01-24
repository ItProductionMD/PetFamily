using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using static PetFamily.Domain.Shared.ValueObjects.ValueObjectFactory;
using static PetFamily.Application.Validations.ValidationExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using System.Security.AccessControl;

//-------------------------------Handler,UseCases,Services----------------------------------------//

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerRepository volunteerRepository,
    IValidator<CreateVolunteerRequest> validator,
    ILogger<CreateVolunteerHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly IValidator<CreateVolunteerRequest> _validator = validator;
    private readonly ILogger<CreateVolunteerHandler> _logger = logger;

    public async Task<Result<Guid>> Handler(
        CreateVolunteerRequest volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        //--------------------------------------Validator----------------------------------//

        /*var validationResult = VolunteerRequestValidator.Validate(volunteerRequest);

        if (validationResult.IsFailure)
        {
            _logger.LogError("CreateVolunteerHandler volunteerRequest validation failure!{}",
                validationResult.Errors);

            return Result<Guid>.Failure(validationResult.Errors!);
        }
        */
        //--------------------------------------Fluent Validator----------------------------------//

        var fluentValidationResult = await _validator.ValidateAsync(volunteerRequest, cancellationToken);

        if (fluentValidationResult.IsValid == false)
        {
            _logger.LogError("Validate volunteerRequest failure!{}",
                           fluentValidationResult.Errors);

            return fluentValidationResult.Failure<Guid>();
        }

        //------------------Creating ValueObject lists from VolunteerRequest Dtos-----------------//

        IReadOnlyList<SocialNetwork> socialNetworks = MapDtosToValueObjects(
            volunteerRequest.SocialNetworksDtos,
            dto => SocialNetwork.Create(dto.Name, dto.Url))!;

        IReadOnlyList<DonateDetails> donateDetails = MapDtosToValueObjects(
            volunteerRequest.DonateDetailsDtos,
            dto => DonateDetails.Create(dto.Name, dto.Description))!;

        //-------------------------------Creating ValueObjects------------------------------------//

        var fullName = FullName.Create(volunteerRequest.FirstName, volunteerRequest.LastName).Data;

        var phone = Phone.Create(volunteerRequest.PhoneNumber, volunteerRequest.PhoneRegionCode).Data;

        //---------------------------------Create Volunteer---------------------------------------//

        var volunteerCreateResult = Volunteer.Create(
            VolunteerID.NewGuid(),
            fullName,
            volunteerRequest.Email,
            phone,
            volunteerRequest.ExperienceYears,
            volunteerRequest.Description,
            donateDetails,
            socialNetworks
            );

        if (volunteerCreateResult.IsFailure)
        {
            _logger.LogError("Validate volunteer entity failure!{}", volunteerCreateResult.Errors);

            return Result<Guid>.Failure(volunteerCreateResult.Errors!);
        }
        var volunteer = volunteerCreateResult.Data;

        //TODO this must be a transaction ?

        //---------------------Check if volunteer with souch Email or phone number exists---------//
        var validateUniqueness = await ValidateUniqueEmailAndPhone(volunteer);

        if (validateUniqueness.IsFailure)
        {
            _logger.LogError(
                "Validate volunteer unique phone and unique email failure!{}",
                validateUniqueness.Errors);

            return Result<Guid>.Failure(validateUniqueness.Errors!);
        }

        //---------------------------------Add Volunteer to repository----------------------------//

        var idResult = await _volunteerRepository.Add(volunteerCreateResult.Data, cancellationToken);

        if (idResult.IsFailure)
        {
            _logger.LogError("Add volunteer with id:{} to repository failure!{}",
                volunteer.Id,
                idResult.Errors);

            return idResult;
        }
        _logger.LogInformation("Volunteer with id:{} , created sucessfull!", volunteer.Id);

        return Result<Guid>.Success(idResult.Data);
    }

    private async Task<Result> ValidateUniqueEmailAndPhone(Volunteer volunteer)
    {
        var getVolunteer = await _volunteerRepository
            .GetByEmailOrPhone(volunteer.Email,volunteer.PhoneNumber);

        if (getVolunteer.IsFailure)
            return Result.Success();

        var existingVolunteers = getVolunteer.Data;

        List<Error> errors = [];

        foreach (var v in existingVolunteers)
        {
            if (v.Email == volunteer.Email)
                errors.Add(Error.CreateErrorValueIsBusy("Email"));

            if (v.PhoneNumber == volunteer.PhoneNumber)
                errors.Add(Error.CreateErrorValueIsBusy("Phone"));
        }
        return Result.Failure(errors!);
    }
}
