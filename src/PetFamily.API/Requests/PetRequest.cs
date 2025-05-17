using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Application.Commands.PetManagment.UpdatePet;
using PetFamily.Application.Dtos;

namespace PetFamily.API.Requests;

public record PetRequest(
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
    public AddPetCommand ToAddPetCommand(Guid volunteerId) =>
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

    public UpdatePetCommand ToUpdatePetCommand(Guid volunteerId, Guid petId) =>
       new UpdatePetCommand(
          volunteerId,
          petId,
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
