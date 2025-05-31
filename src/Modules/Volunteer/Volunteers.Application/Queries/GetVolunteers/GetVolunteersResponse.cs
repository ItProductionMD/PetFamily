using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Queries.GetVolunteers;

public record GetVolunteersResponse(int VolunteersCount, List<VolunteerMainInfoDto> Volunteers);

