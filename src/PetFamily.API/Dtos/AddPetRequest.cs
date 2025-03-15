using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Application.Commands.PetManagment.Dtos;

namespace PetFamily.API.Dtos;

public record AddPetRequest(
   string PetName,
   DateOnly? DateOfBirth,
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
   IEnumerable<RequisitesDto> DonateDetails)
{
    public AddPetCommand ToCommand(Guid volunteerId) =>
        new AddPetCommand(
           volunteerId,
           PetName,
           DateOfBirth,
           Description,
           IsVaccinated,
           IsSterilized,
           Weight,
           Height,
           Color,
           SpeciesId,
           BreedId,
           OwnerPhoneRegion,
           OwnerPhoneNumber,
           HealthInfo,
           HelpStatus,
           City,
           Region,
           Street,
           HomeNumber,
           DonateDetails);   
}   
