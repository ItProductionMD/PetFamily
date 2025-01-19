using FluentValidation;
using PetFamily.Application.Validations;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using static PetFamily.Domain.Shared.ValueObjects.ValueObjectFactory;
using static PetFamily.Application.Validations.ValidationExtensions;

//-------------------------------Handler,UseCases,Services----------------------------------------//

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler(
    IVolunteerRepository volunteerRepository,
    IValidator<CreateVolunteerRequest> validator)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly IValidator<CreateVolunteerRequest> _validator = validator;
    public async Task<Result<Guid>> Handler(
        CreateVolunteerRequest volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        //--------------------------------------Validator----------------------------------//

        var validationResult = VolunteerRequestValidator.Validate(volunteerRequest);

        if (validationResult.IsFailure)
            return Result<Guid>.Failure(validationResult.Errors!);

        //--------------------------------------Fluent Validator----------------------------------//

        var fluentValidationResult = await _validator.ValidateAsync(volunteerRequest, cancellationToken);

        if (fluentValidationResult.IsValid == false)
            return fluentValidationResult.Failure<Guid>();

        //------------------Creating ValueObject lists from VolunteerRequest Dtos-----------------//

        IReadOnlyList<SocialNetwork> socialNetworks = MapDtosToValueObjects(
            volunteerRequest.SocialNetworksDtos,
            dto => SocialNetwork.Create(dto.Name, dto.Url))!;

        IReadOnlyList<DonateDetails> donateDetails = MapDtosToValueObjects(
            volunteerRequest.DonateDetailsDtos,
            dto => DonateDetails.Create(dto.Name, dto.Description))!;

        //-------------------------------Creating ValueObjects------------------------------------//

        var fullNameResult = FullName.Create(volunteerRequest.FirstName, volunteerRequest.LastName);

        var phoneResult = Phone.Create(volunteerRequest.PhoneNumber, volunteerRequest.PhoneRegionCode);

        //---------------------------------Create Volunteer---------------------------------------//

        var volunteerCreateResult = Volunteer.Create(
            VolunteerID.NewGuid(),
            fullNameResult.Data,
            volunteerRequest.Email,
            phoneResult.Data,
            volunteerRequest.ExperienceYears,
            volunteerRequest.Description,
            donateDetails,
            socialNetworks
            );

        if (volunteerCreateResult.IsFailure)
            return Result<Guid>.Failure(volunteerCreateResult.Errors!);

        //---------------------------------Add Volunteer to repository----------------------------//

        var idResult = await _volunteerRepository.Add(volunteerCreateResult.Data, cancellationToken);

        if (idResult.IsFailure)
            return idResult;

        return Result<Guid>.Success(idResult.Data);
    }

}
