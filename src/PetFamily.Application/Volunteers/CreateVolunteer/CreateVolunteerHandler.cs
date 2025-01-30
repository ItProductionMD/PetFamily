using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using static PetFamily.Domain.Shared.ValueObjects.ValueObjectFactory;
using static PetFamily.Application.Validations.ValidationExtensions;
using static PetFamily.Application.Volunteers.VolunteerValidationExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.Shared;
using System.Security.AccessControl;


//-------------------------------Handler,UseCases,Services----------------------------------------//
namespace PetFamily.Application.Volunteers.CreateVolunteer;
/// <summary>
/// Create Volunteer Handler
/// </summary>
/// <param name="volunteerRepository"></param>
/// <param name="validator"></param>
/// <param name="logger"></param>
public class CreateVolunteerHandler(
    IVolunteerRepository volunteerRepository,
    IValidator<CreateVolunteerCommand> validator,
    ILogger<CreateVolunteerHandler> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly IValidator<CreateVolunteerCommand> _validator = validator;
    private readonly ILogger<CreateVolunteerHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(
        CreateVolunteerCommand volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        //--------------------------------------Validator----------------------------------//

        /*var validationResult = VolunteerRequestValidatorCustom.Validate(volunteerRequest);

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
            _logger.LogError("Validate volunteerRequest failure!{fluentValidationResult.Errors}",
                           fluentValidationResult.Errors);

            return fluentValidationResult.ToResultFailure<Guid>();
        }

        //------------------Creating ValueObject lists from VolunteerRequest Dtos-----------------//

        IReadOnlyList<SocialNetwork> socialNetworks = MapDtosToValueObjects(
            volunteerRequest.SocialNetworksList,
            dto => SocialNetwork.Create(dto.Name, dto.Url))!;

        IReadOnlyList<DonateDetails> donateDetails = MapDtosToValueObjects(
            volunteerRequest.DonateDetailsList,
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
            _logger.LogError(
                "Validate volunteer entity failure!{ volunteerCreateResult.Errors}",
                volunteerCreateResult.Errors);

            return Result<Guid>.Failure(volunteerCreateResult.Errors!);
        }
        var volunteer = volunteerCreateResult.Data;

        //---------------------Verify that the email and phone number are unique------------------//
        var validateUniqueness = await ValidateUniqueEmailAndPhone(
            volunteer,
            _volunteerRepository,
            _logger,
            cancellationToken);

        if (validateUniqueness.IsFailure)
            return Result<Guid>.Failure(validateUniqueness.Errors!);
        
        //---------------------------------Add Volunteer to repository----------------------------//
        var idResult = await _volunteerRepository.Add(volunteerCreateResult.Data, cancellationToken);

        if (idResult.IsFailure)
        {
            _logger.LogError(
                "Add volunteer with id:{volunteer.Id} to repository failure!{idResult.Errors}",
                volunteer.Id,
                idResult.Errors);

            return idResult;
        }
        _logger.LogInformation("Volunteer with id:{volunteer.Id},created sucessfull!", volunteer.Id);

        return Result<Guid>.Success(idResult.Data);
    }
}
