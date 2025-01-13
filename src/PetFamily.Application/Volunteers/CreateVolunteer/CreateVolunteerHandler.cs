using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
//Handler,UseCases,Services
namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteerRepository _volunteerRepository;
    public CreateVolunteerHandler(IVolunteerRepository volunteerRepository)
    {
        _volunteerRepository= volunteerRepository;
    }
    public async Task<Result<Guid>> Handler(CreateVolunteerRequest volunteerRequest,CancellationToken cancellationToken=default)
    {
        var volunteerId=VolunteerID.NewGuid();
        var expirienceYears = volunteerRequest.ExpirienceYears;
        var email=volunteerRequest.Email;
        var description = volunteerRequest.Description;

        var fullNameResult = FullName.Create(volunteerRequest.FirstName, volunteerRequest.LastName);
        if (fullNameResult.IsFailure)
            return Result<Guid>.Failure(fullNameResult.Error!);

        var phoneNumberResult = Phone.Create(volunteerRequest.PhoneNumber, volunteerRequest.PhoneRegionCode);
        if (phoneNumberResult.IsFailure)
            return Result<Guid>.Failure(phoneNumberResult.Error!);
        var volunteerCreateResult = Volunteer.Create(
            volunteerId,
            fullNameResult.Data,
            email,
            phoneNumberResult.Data,
            expirienceYears,
            description,
            null,
            null
            );
        if (volunteerCreateResult.IsFailure)
            return Result<Guid>.Failure(volunteerCreateResult.Error!);
        var idResult = await _volunteerRepository.Add(volunteerCreateResult.Data,cancellationToken);
        if (idResult.IsFailure)
            return idResult;
        return Result<Guid>.Success(volunteerId.Value);
    }
}
