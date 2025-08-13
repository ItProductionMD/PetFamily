using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.HTTPContext.Interfaces;
using PetFamily.VolunteerRequests.Application.Commands.ApproveVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;
using PetFamily.VolunteerRequests.Application.Commands.UpdateVolunteerRequest;
using PetFamily.VolunteerRequests.Application.Queries.GetRequest;
using PetFamily.VolunteerRequests.Application.Queries.GetRequestsOnReview;
using PetFamily.VolunteerRequests.Application.Queries.GetUnreviewedRequests;
using PetFamily.VolunteerRequests.Presentation.Requests;
using static PetFamily.Framework.Extensions.ResultExtensions;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.VolunteerRequestManagement;


namespace PetFamily.VolunteerRequests.Presentation.Controllers;

[Route("api/volunteer_requests")]
[ApiController]
public class VolunteerRequestController(IUserContext userContext) : ControllerBase
{
    private readonly IUserContext _userContext = userContext;

    /// <summary>
    /// Creates a new volunteer request.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestCreate)]
    [HttpPost]
    public async Task<ActionResult<Envelope>> Create(
        [FromServices] SubmitVolunteerRequestHandler handler,
        [FromBody] SubmitVolunteerRequest request,
        CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();
        var command = request.ToCommand(userId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Takes a volunteer request for review by an admin.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestTakeForReview)]
    [HttpPost("{id:guid}/take_for_review")]
    public async Task<ActionResult<Envelope>> TakeForReview(
        [FromServices] TakeVolunteerRequestForReviewHandler handler,
        Guid id,
        CancellationToken ct = default)
    {
        var adminId = _userContext.GetUserId();
        var command = new TakeVolunteerRequestForReviewCommand(adminId, id);
        var result = await handler.Handle(command, ct);

        return result.ToCreatedAtActionResult(nameof(Create));
    }

    /// <summary>
    /// Approves a volunteer request.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestApprove)]
    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<Envelope>> Approve(
        [FromServices] ApproveVolunteerRequestHandler handler,
        Guid id,
        CancellationToken ct = default)
    {
        var adminId = _userContext.GetUserId();
        var command = new ApproveVolunteerRequestCommand(adminId, id);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Rejects a volunteer request with an optional comment.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="comment"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestReject)]
    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<Envelope>> Reject(
        [FromServices] RejectVolunteerRequestHandler handler,
        Guid id,
        [FromBody] RejectVolunteerRequest request,
        CancellationToken ct = default)
    {
        var adminId = _userContext.GetUserId();
        var command = request.ToCommand(adminId, id);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Sends a volunteer request back to revision.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestSendToRevision)]
    [HttpPost("{id:guid}/send_to_revision")]
    public async Task<ActionResult<Envelope>> SendToRevision(
        [FromServices] SendRequestToRevisionHandler handler,
        Guid id,
        [FromBody] SendRequestToRevisionRequest request,
        CancellationToken ct = default)
    {
        var adminId = _userContext.GetUserId();
        var command = request.ToCommand(adminId, id);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Updates an existing volunteer request.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestUpdate)]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<Envelope>> UpdateVolunteerRequest(
        [FromServices] UpdateVolunteerRequestHandler handler,
        Guid id,
        [FromBody] UpdateVolunteerRequestRequest request,
        CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();

        var command = request.ToCommand(userId, id);

        return (await handler.Handle(command, ct)).ToActionResult(); ;
    }


    /// <summary>
    /// Retrieves volunteer requests that are currently on review by admins.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="statuses"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestsGetOnReview)]
    [HttpGet("on_review")]
    public async Task<ActionResult<Envelope>> GetRequestsOnReview(
        [FromServices] GetRequestsOnReviewHandler handler,
        [FromQuery] List<string> statuses,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var adminId = _userContext.GetUserId();

        var query = new GetRequestsOnReviewQuery(
            page,
            pageSize,
            new VolunteerRequestsFilter(statuses));

        return (await handler.Handle(query, ct)).ToActionResult();
    }


    /// <summary>
    /// Retrieves unreviewed volunteer requests for admins.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestsGetUnreviewed)]
    [HttpGet("unreviewed")]
    public async Task<ActionResult<Envelope>> GetUnreviewedRequests(
        [FromServices] GetUnreviewedRequestsHandler handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetUnreviewedRequestsQuery(page, pageSize);

        return (await handler.Handle(query, ct)).ToActionResult();
    }

    /// <summary>
    ///     Retrieves the volunteer request made by the current user.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRequestView)]
    [HttpGet("own_request")]
    public async Task<ActionResult<Envelope>> GetOwnRequest(
        [FromServices] GetOwnVolunteerRequestHandler handler,
        CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();
        var query = new GetOwnVolunteerRequestQuery(userId);

        return (await handler.Handle(query, ct)).ToActionResult(); ;
    }
}

