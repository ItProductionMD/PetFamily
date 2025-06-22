namespace Volunteers.Application.ResponseDtos;

public record VolunteerMainInfoDto(
    Guid Id, 
    Guid UserId, 
    string FullName,
    string Phone, 
    int Rating,
    List<RequisitesDto> RequisitesDtos);
