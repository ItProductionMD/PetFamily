using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteVolunteer;
using PetFamily.Application.Volunteers.GetVolunteer;
using PetFamily.Application.Volunteers.RestoreVolunteer;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateVolunteer;
using static PetFamily.API.Extensions.ResultExtensions;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteerController : Controller
{
    private readonly ILogger<VolunteerController> _logger;
    public VolunteerController(ILogger<VolunteerController> logger)
    {
        _logger = logger;
    }
    //------------------------------------CreateVolunteer-----------------------------------------//
    /// <summary>
    /// Create a volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Envelope>> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerCommand volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(volunteerRequest, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "Create volunteer failure!{handlerResult.Errors}",
                handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation(
            "Create volunteer with id:{handlerResult.Data} success!",
            handlerResult.Data);

        return CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }
    //------------------------------------UpdateVolunteer-----------------------------------------//
    /// <summary>
    /// Update a volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="dto"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> Code:200 </returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult<Envelope>> Update(
        [FromServices] UpdateVolunteerHandler handler,
        [FromBody] VolunteerUpdateDto dto,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var volunteerRequest = new UpdateVolunteerRequest(
            id,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Description,
            dto.PhoneNumber,
            dto.PhoneRegionCode,
            dto.ExperienceYears);

        var handlerResult = await handler.Handle(volunteerRequest, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "Update volunteer with id:{id} failure!{handlerResult.Errors}",
                id,
                handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation("Update volunteer  with id:{id} success!", id);

        return Ok(handlerResult.ToEnvelope());
    }
    //--------------------------------------Get Volunteer ById------------------------------------//
    /// <summary>
    /// Get a volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Envelope>> Get(
        [FromServices] GetVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(id, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "Get volunteer with id:{id} failure!{handlerResult.Errors}",
                id,
                handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation("Get volunteer with id:{id} success!", id);

        return Ok(handlerResult.ToEnvelope());
    }
    //------------------------------------SoftDeleteVolunteer-------------------------------------//
    /// <summary>
    /// Soft Delete Volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}/soft")]
    public async Task<ActionResult<Envelope>> SoftDelete(
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(id, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "SoftDelete volunteer with id:{id} failure!{handlerResult.Error}",
                id,
                handlerResult.Errors);

            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation("SoftDelete volunteer with id:{id} success!", id);

        return Ok(handlerResult.ToEnvelope());
    }
    //------------------------------------HardDeleteVolunteer-------------------------------------//
    /// <summary>
    ///  Hard Delete Volunteer  
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}/hard")]
    public async Task<ActionResult<Envelope>> Delete(
       [FromServices] DeleteVolunteerHandler handler,
       [FromRoute] Guid id,
       CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(id, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "Delete volunteer with id:{id} failure!{handlerResult.Errors}",
                 id,
                 handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation("Delete volunteer with id:{id} success!", id);

        return Ok(handlerResult.ToEnvelope());
    }
    //------------------------------------UpdateSocialNetworks------------------------------------//
    /// <summary>
    /// Update SocialNetworks
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="dtos"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}/social_networks")]
    public async Task<ActionResult<Envelope>> UpdateSocialNetworks(
        [FromServices] UpdateSocialNetworkHandler handler,
        [FromBody] IEnumerable<SocialNetworksRequest> dtos,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(id, dtos, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "Update social networks for volunteer with id:{id} failure!{handlerResult.Errors}",
                id,
                handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation("Update social networks for volunteer with id:{id} success!", id);

        return Ok(handlerResult.ToEnvelope());
    }
    //------------------------------------RestoreVolunteer----------------------------------------//
    /// <summary>
    /// Restore volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}/restore")]
    public async Task<ActionResult<Envelope>> Restore(
        [FromServices] RestoreVolunteerHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(id, cancellationToken);
        if (handlerResult.IsFailure)
        {
            _logger.LogError(
                "Restore volunteer with id:{id} failure!{handlerResult.Errors}",
                id, 
                handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }
        _logger.LogInformation("Restore volunteer with id:{id} success!", id);

        return Ok(handlerResult.ToEnvelope());
    }
}
