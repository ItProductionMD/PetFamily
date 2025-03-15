using PetFamily.Application.Dtos;

namespace PetFamily.Application.Queries;

public record GetVolunteersResponse(int VolunteersCount,List<VolunteerMainInfoDto> Volunteers);

