using PetFamily.Application.Volunteers.Dtos;

namespace PetFamily.Application.Volunteers.GetVolunteers;

public record GetVolunteersResponse(int ItemCounts,IEnumerable<VolunteerDto> Dtos);

