using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.VolunteerRequest.Presentation.Requests;
using PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;
using PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;
using static PetFamily.Framework.Extensions.ResultExtensions;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.VolunteerManagement;

namespace PetFamily.VolunteerRequest.Presentation.Controllers;

[Route("api/volunteer_requests")]
[ApiController]

public class VolunteerRequestController : ControllerBase
{
    [Authorize(Policy = VolunteerCreate)]
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

    [HttpPost("{id:guid}/take_for_review")]
    public async Task<ActionResult<Envelope>> TakeForReview(
        [FromServices] TakeVolunteerRequestForReviewHandler handler,
        Guid id,
        CancellationToken ct = default)
    {
        var command = new TakeVolunteerRequestForReviewCommand(id);
        var handlerResult = await handler.Handle(command, ct);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<Envelope>> Approve(
        [FromServices] ApproveVolunteerRequestHandler handler,
        Guid id,
        CancellationToken ct = default)
    {
        var command = new ApproveVolunteerRequestCommand(id);
        var handlerResult = await handler.Handle(command, ct);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<Envelope>> Reject(
        [FromServices] RejectVolunteerRequestHandler handler,
        Guid id,
        [FromBody] string comment,
        CancellationToken ct = default)
    {
        var command = new RejectVolunteerRequestCommand(id, comment);

        var handlerResult = await handler.Handle(command, ct);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }


    [HttpPost("{id:guid}/send_to_revision")]
    public async Task<ActionResult<Envelope>> SendToRevision(
        [FromServices] SendRequestToRevisionHandler handler,
        Guid id,
        [FromBody] string comment,
        CancellationToken ct = default)
    {
        var command = new SendRequestToRevisionCommand(id, comment);

        var handlerResult = await handler.Handle(command, ct);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<Envelope>> UpdateVolunteerRequest(
        [FromServices] UpdateVolunteerRequestHandler handler,
        Guid id,
        [FromBody] UpdateVolunteerRequestDto dto,
        CancellationToken ct = default)
    {
        var command = new UpdateVolunteerRequestCommand(
            id,
            dto.LastName,
            dto.FirstName,
            dto.Description,
            dto.DocumentName,
            dto.ExperienceYears,
            dto.Requisites);

        var handlerResult = await handler.Handle(command, ct);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }
}

