using PetFamily.Domain.Shared.DomainResult;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using System.Net.Http.Headers;

//Handler,UseCases,Services
namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteerRepository _volunteerRepository;

    public CreateVolunteerHandler(IVolunteerRepository volunteerRepository)
    {
        _volunteerRepository= volunteerRepository;
    }

    public async Task<Result<Guid>> Handler(
        CreateVolunteerRequest volunteerRequest,
        CancellationToken cancellationToken=default)
    {

        var volunteerId=VolunteerID.NewGuid();
        var expirienceYears = volunteerRequest.ExpirienceYears;
        var email=volunteerRequest.Email;
        var description = volunteerRequest.Description;

        var donateDetailsResult = CreateDonateDetailsList(volunteerRequest.DonateDetailsDtos);
        if (donateDetailsResult.IsFailure)
            return Result<Guid>.Failure(donateDetailsResult.Error!);

        var socialNetworksResult = CreateSocialNetworkList(volunteerRequest.SocialNetworksDtos);
        if(socialNetworksResult.IsFailure)
            return Result<Guid>.Failure(socialNetworksResult.Error!);

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
            donateDetailsResult.Data,
            socialNetworksResult.Data
            );

        if (volunteerCreateResult.IsFailure)
            return Result<Guid>.Failure(volunteerCreateResult.Error!);

        var idResult = await _volunteerRepository.Add(volunteerCreateResult.Data,cancellationToken);
        if (idResult.IsFailure)
            return idResult;

        return Result<Guid>.Success(volunteerId.Value);
    }

    private Result<List<DonateDetails>> CreateDonateDetailsList(List<DonateDetailsDto> donateDetailsDtos)
    {
        List<DonateDetails> donateDetailsList = [];

        foreach (var item in donateDetailsDtos)
        {
            var donateDetailsResult = DonateDetails.Create(item.Name, item.Description,false);

            if (donateDetailsResult.IsFailure)
                return Result<List<DonateDetails>>.Failure(donateDetailsResult.Error!);
                    
            if(donateDetailsResult.Data != null)
                donateDetailsList.Add(donateDetailsResult.Data);
        }

        return Result<List<DonateDetails>>.Success(donateDetailsList);
    }

    private Result<List<SocialNetwork>> CreateSocialNetworkList(List<SocialNetworksDto> socialNetworksDtos)
    {
        List<SocialNetwork> socialNetworks = [];

        foreach(var item in socialNetworksDtos)
        {
            var socialNetworksResult = SocialNetwork.Create(item.Name,item.Url,false);

            if(socialNetworksResult.IsFailure)
                return Result<List<SocialNetwork>>.Failure(socialNetworksResult.Error!);

            if(socialNetworksResult.Data != null)   
                socialNetworks.Add(socialNetworksResult.Data);
        }

        return Result<List<SocialNetwork>>.Success(socialNetworks);
    }
}
