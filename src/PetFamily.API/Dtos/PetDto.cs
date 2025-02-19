using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.API.Dtos;

public record PetDto(
   string PetName,
   DateOnly DateOfBirth,
   string Description,
   bool IsVaccinated,
   bool IsSterilized,
   double Weight,
   double Height,
   string Color,
   Guid SpeciesId,
   Guid BreedId,
   string OwnerPhoneRegion,
   string OwnerPhoneNumber,
   string HealthInfo,
   int HelpStatus,
   string City,
   string Region,
   string Street,
   string HomeNumber,
   IEnumerable<RequisitesRequest> DonateDetails);   
