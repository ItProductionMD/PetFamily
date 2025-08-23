using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.Discussions.Application.Commands.CreateDiscussion;
using PetFamily.Discussions.Application.Commands.DeleteDiscussionMessage;
using PetFamily.Discussions.Application.Commands.LeaveMessage;
using PetFamily.Discussions.Application.Commands.UpdateMessage;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.Discussions.Presentation.Requests;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Framework.HTTPContext.User;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.DiscussionManagement;

namespace PetFamily.Discussions.Presentation.Controllers;

[ApiController]
[Route("api/discussions")]
public class DiscussionController(IUserContext userContext) : Controller
{

    /// <summary>
    /// Gets a discussion by its ID.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="discussionId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = DiscussionMessageView)]
    [HttpGet("{discussionId:guid}")]
    public async Task<ActionResult<Envelope>> GetDiscussion(
        [FromServices] GetDiscussionHandler handler,
        Guid discussionId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var query = new GetDiscussionQuery(userId, discussionId, page, pageSize);

        return (await handler.Handle(query, ct)).ToActionResult();
    }

    /// <summary>
    /// Closes a discussion by its ID.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="discussionId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPatch("{discussionId:guid}/close")]
    [Authorize(Policy = DiscussionClose)]
    public async Task<ActionResult<Envelope>> CloseDiscussion(
        [FromServices] CloseDiscussionHandler handler,
        Guid discussionId,
        CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var command = new CloseDiscussionCommand(userId, discussionId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Envelope>> CreateDiscussion(
        [FromServices] CreateDiscussionHandler handler,
        [FromBody] CreateDiscussionRequest request,
        CancellationToken ct = default)
    {
        var adminId = userContext.GetUserId();
        var command = request.ToCommand(adminId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Creates a new discussion message.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="discussionId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("{discussionId:guid}/messages")]
    [Authorize(Policy = DiscussionMessageCreate)]
    public async Task<ActionResult<Envelope>> LeaveDiscussionMessage(
        [FromServices] LeaveDiscussionMessageHandler handler,
        Guid discussionId,
        [FromBody] DiscussionMessageRequest request,
        CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var command = request.ToLeaveCommand(userId, discussionId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Updates an existing discussion message.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="discussionId"></param>
    /// <param name="messageId"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPatch("{discussionId:guid}/messages/{messageId:guid}")]
    [Authorize(Policy = DiscussionMessageEdit)]
    public async Task<ActionResult<Envelope>> UpdateDiscussionMessage(
        [FromServices] UpdateDiscussionMessageHandler handler,
        Guid discussionId,
        Guid messageId,
        [FromBody] DiscussionMessageRequest request,
        CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var command = request.ToUpdateCommand(userId, discussionId, messageId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Deletes a discussion message.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="discussionId"></param>
    /// <param name="messageId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpDelete("{discussionId:guid}/messages/{messageId:guid}")]
    [Authorize(Policy = DiscussionMessageDelete)]
    public async Task<ActionResult<Envelope>> DeleteDiscussionMessage(
       [FromServices] DeleteDiscussionMessageHandler handler,
       Guid discussionId,
       Guid messageId,
       CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var command = new DeleteDiscussionMessageCommand(userId, discussionId, messageId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }
}
