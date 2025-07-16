using PetFamily.SharedApplication.Dtos;

namespace Volunteers.Application.ResponseDtos;

public class VolunteerDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Phone { get; set; }
    public int Rating { get; set; }
    public IReadOnlyList<RequisitesDto> RequisitesDtos { get; set; }
    public List<PetMainInfoDto> PetDtos { get; set; } = [];

    public VolunteerDto(
        Guid id,
        Guid userId,
        string lastName,
        string firstName,
        string phone,
        int rating,
        List<RequisitesDto> requisitesDtos,
        List<PetMainInfoDto>? petDtos)
    {
        Id = id;
        UserId = userId;
        LastName = lastName;
        FirstName = firstName;
        Phone = phone;
        RequisitesDtos = requisitesDtos;
        PetDtos = petDtos ?? [];
        Rating = rating;
    }
}