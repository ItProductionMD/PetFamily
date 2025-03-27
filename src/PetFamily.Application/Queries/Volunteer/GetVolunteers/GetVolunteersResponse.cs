using PetFamily.Application.Dtos;

namespace PetFamily.Application.Queries.Volunteer.GetVolunteers;

public record GetVolunteersResponse(int VolunteersCount,List<VolunteerMainInfoDto> Volunteers);

