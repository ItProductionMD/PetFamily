using Microsoft.Extensions.Diagnostics.HealthChecks;
using PetFamily.SharedApplication.Dtos;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Presentation.Requests;

public record CreateVolunteerRequest
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string PhoneRegionCode { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public int ExperienceYears { get; set; } 
    public IEnumerable<RequisitesDto> Requisites { get; set; } = [];

    public CreateVolunteerCommand ToCommand(Guid adminId) =>
        new(
            adminId,
            UserId,
            FirstName,
            LastName,
            Description,    
            ExperienceYears,
            PhoneRegionCode,
            PhoneNumber,
            Requisites
        );
}

