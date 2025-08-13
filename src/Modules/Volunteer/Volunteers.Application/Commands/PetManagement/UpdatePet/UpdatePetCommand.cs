using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedApplication.Dtos;

namespace Volunteers.Application.Commands.PetManagement.UpdatePet;

public record UpdatePetCommand(
   Guid VolunteerId,
   Guid PetId,
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
   string PhoneRegion,
   string PhoneNumber,
   string HealthInfo,
   int HelpStatus,
   string City,
   string Region,
   string Street,
   string HomeNumber,
   IEnumerable<RequisitesDto> Requisites) : ICommand;

