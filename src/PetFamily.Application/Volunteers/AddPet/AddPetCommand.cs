using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.Application.Volunteers.AddPet
{
    public record AddPetCommand(
       Guid VolunteerId,
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
}
