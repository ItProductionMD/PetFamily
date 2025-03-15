using PetFamily.Application.Commands.PetManagment.Dtos;

namespace PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;

public record VolunteersResponse(int ItemCounts,IEnumerable<VolunteerDto> Dtos);

