using Microsoft.AspNetCore.Mvc;
using PetFamily.Discussions.Presentation.Requests;
using PetFamily.Discussions.Application.Commands.CloseDiscussion;
using PetFamily.Discussions.Application.Commands.CreateDiscussion;
using PetFamily.Discussions.Application.Commands.DeleteDiscussionMessage;
using PetFamily.Discussions.Application.Commands.LeaveMessage;
using PetFamily.Discussions.Application.Commands.UpdateMessage;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.SharedApplication.IUserContext;

namespace PetFamily.Discussions.Presentation.Controllers;

[ApiController]
[Route("api/discussions")]
public class DiscussionController(
    IUserContext userContext) : Controller
{
    private readonly IUserContext _userContext = userContext;

    [HttpGet("{discussionId:guid}")]
    public async Task<ActionResult<Envelope>> GetDiscussion(
        [FromServices] GetDiscussionHandler handler,
        Guid discussionId,
        int page = 1,
        int pageSize = 10,
        CancellationToken ct = default) 
    {
        var userId = _userContext.GetUserId();
        var query = new GetDiscussionQuery(userId, discussionId, page, pageSize);

        return (await handler.Handle(query, ct)).ToActionResult();
    }

    [HttpPatch("{discussionId:guid}/close")]
    public async Task<ActionResult<Envelope>> CloseDiscussion(
        [FromServices] CloseDiscussionHandler handler,
        Guid discussionId,
        CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();
        var command = new CloseDiscussionCommand(userId, discussionId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Envelope>> CreateDiscussion(
        [FromServices] CreateDiscussionHandler handler,
        [FromBody] CreateDiscussionRequest request,
        CancellationToken ct = default)
    {
        var adminId = _userContext.GetUserId();
        var command = request.ToCommand(adminId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    [HttpPost("{discussionId:guid}/messages")]
    public async Task<ActionResult<Envelope>> LeaveDiscussionMessage(
        [FromServices] LeaveDiscussionMessageHandler handler,
        Guid discussionId,
        [FromBody]DiscussionMessageRequest request,
        CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();
        var command = request.ToLeaveCommand(userId, discussionId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    [HttpPatch("{discussionId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<Envelope>> UpdateDiscussionMessage(
        [FromServices] UpdateDiscussionMessageHandler handler,
        Guid discussionId,
        Guid messageId,
        [FromBody] DiscussionMessageRequest request,
        CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();
        var command = request.ToUpdateCommand(userId, discussionId, messageId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    [HttpDelete("{discussionId:guid}/messages/{messageId:guid}")]
    public async Task<ActionResult<Envelope>> DeleteDiscussionMessage(
       [FromServices] DeleteDiscussionMessageHandler handler,
       Guid discussionId,
       Guid messageId,
       CancellationToken ct = default)
    {
        var userId = _userContext.GetUserId();
        var command = new DeleteDiscussionMessageCommand(userId, discussionId, messageId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }
}
