using PetFamily.Application.Abstractions;
using PetFamily.Application.Dtos;

namespace PetFamily.Application.Commands.PetManagment.AddPet;

public record AddPetCommand(
   Guid VolunteerId,
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
   IEnumerable<RequisitesDto> Requisites) : ICommand;
