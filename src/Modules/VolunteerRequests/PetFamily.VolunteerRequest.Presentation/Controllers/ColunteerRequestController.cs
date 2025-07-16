using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.SharedApplication.Dtos;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;
using static PetFamily.Framework.Extensions.ResultExtensions;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.VolunteerManagement;

namespace PetFamily.VolunteerRequest.Presentation.Controllers;

[Route("api/volunteer_requests")]
[ApiController]

public class ColunteerRequestController : ControllerBase
{
    //[Authorize(Policy = VolunteerCreate)]
    [HttpPost]
    public async Task<ActionResult<Envelope>> Create(
        [FromServices] SubmitVolunteerRequestHandler handler,
        [FromBody] SubmitVolunteerRequestDto dto,
        CancellationToken ct = default)
    {
        var command = new SubmitVolunteerRequestCommand(
            dto.DocumentName,
            dto.LastName,
            dto.FirstName,
            dto.Description,
            dto.ExperienceYears,
            dto.Requisites);

        var handlerResult = await handler.Handle(command, ct);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }
}


public record SubmitVolunteerRequestDto(
    string DocumentName,
    string LastName,
    string FirstName,
    string Description,
    int ExperienceYears,
    IEnumerable<RequisitesDto> Requisites);
