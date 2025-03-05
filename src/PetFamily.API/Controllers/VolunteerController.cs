using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.FilesManagment.Commands;
using PetFamily.Application.SharedValidations;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteVolunteer;
using PetFamily.Application.Volunteers.GetVolunteer;
using PetFamily.Application.Volunteers.RestoreVolunteer;
using PetFamily.Application.Volunteers.UpdateRequisites;
using PetFamily.Application.Volunteers.UpdateSocialNetworks;
using PetFamily.Application.Volunteers.UpdateVolunteer;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using static PetFamily.API.Extensions.ResultExtensions;
using static PetFamily.API.AppiValidators.Validators;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.UpdatePetImages;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Application.Volunteers.ChangePetPosition;
using PetFamily.API.Common.Utilities;
using PetFamily.Application.Volunteers.Dtos;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteerController(
    IOptions<FileValidatorOptions> validateFileOptions,
    ILogger<VolunteerController> logger) : Controller
{
    private readonly FileValidatorOptions _fileValidatorOptions = validateFileOptions.Value;
    private readonly ILogger<VolunteerController> _logger = logger;
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
        [FromBody] CreateVolunteerRequest volunteerRequest,
        CancellationToken cancellationToken = default)
    {
        var command = volunteerRequest.ToCommand();
        
        var handlerResult = await handler.Handle(command, cancellationToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
    }

    //------------------------------------UpdateVolunteer-----------------------------------------//
    /// <summary>
    /// Update a volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="request"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> Code:200 </returns>
    [HttpPatch("{volunteerId}")]
    public async Task<ActionResult<Envelope>> Update(
        [FromServices] UpdateVolunteerHandler handler,
        [FromBody] UpdateVolunteerRequest request,
        [FromRoute] Guid volunteerId,
        CancellationToken cancellationToken = default)
    {
        var command = request.ToCommand(volunteerId);
            
        var handlerResult = await handler.Handle(command, cancellationToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
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

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
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

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
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

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }
    //------------------------------------UpdateSocialNetworks------------------------------------//
    /// <summary>
    /// Update SocialNetworks
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="dtos"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{volunteerId}/social_networks")]
    public async Task<ActionResult<Envelope>> UpdateSocialNetworks(
        [FromServices] UpdateSocialNetworkHandler handler,
        [FromBody] IEnumerable<SocialNetworksDto> dtos,
        [FromRoute] Guid volunteerId,
        CancellationToken cancellationToken = default)
    {
        var command =  new UpdateSocialNetworksCommand(volunteerId,dtos);

        var handlerResult = await handler.Handle(command,cancellationToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    /// <summary>
    /// Update Requisites
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="dtos"></param>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}/requisites")]
    public async Task<ActionResult<Envelope>> UpdateRequisites(
        [FromServices] UpdateRequisitesHandler handler,
        [FromBody] IEnumerable<RequisitesDto> dtos,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var handlerResult = await handler.Handle(id, dtos, cancellationToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
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

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //-------------------------------------AddPet---------------------------------------------//
    /// <summary>
    /// Adds a new pet for a volunteer.
    /// </summary>
    /// <param name="handler">The handler responsible for processing the request.</param>
    /// <param name="volunteerId">The ID of the volunteer who owns the pet.</param>
    /// <param name="request">The DTO containing pet details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// Returns:
    /// - **200 OK** with an `Envelope<AddPetResponse>` if the operation is successful.
    /// - **400 Bad Request** if the request is invalid or contains errors.
    /// - **500 Internal Server Error** if an unexpected error occurs.
    /// </returns>
    [HttpPost("{volunteerId:Guid}/pet")]
    public async Task<ActionResult<Envelope>> Add(
        [FromServices] AddPetHandler handler,
        [FromRoute] Guid volunteerId,
        [FromBody] AddPetRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(volunteerId);

        var addPetResult = await handler.Handle(command, cancellationToken);

        return addPetResult.IsFailure
            ? addPetResult.ToErrorActionResult()
            : Ok(addPetResult.ToEnvelope());
    }

    //------------------------------------Update pet images---------------------------------------//
    /// <summary>
    /// Update pet images
    /// </summary>
    /// <param name="handler">Handler for updating pet images</param>
    /// <param name="petId">The unique identifier of the pet</param>
    /// <param name="imagesRequest">Request containing images to upload and delete</param>
    /// <param name="cancelToken">Token to cancel the request</param>
    /// <returns>
    /// Returns an <see cref="Envelope"/> containing the update result:
    /// - 200 OK with the updated pet images if successful.
    /// - 400 Bad Request if validation fails.
    /// - 500 Internal Server Error in case of an unexpected error.
    /// </returns>
    [HttpPost("{volunteerId:Guid}/pet/{petId:Guid}/images")]
    public async Task<ActionResult<Envelope>> UpdateImages(
        [FromServices] UpdatePetImagesHandler handler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] UpdateImagesRequest imagesRequest,
        CancellationToken cancelToken)
    {
        if (imagesRequest.ImagesToUpload.Count > Pet.MAX_IMAGES_COUNT)
        {
            _logger.LogWarning(
            "UpdateImages failed: Upload file count ({Count}) exceeds the maximum allowed" +
            " ({MAX_IMAGES_COUNT})",
            imagesRequest.ImagesToUpload.Count, Pet.MAX_IMAGES_COUNT);

            return Result.Fail(Error.InvalidLength("Upload images count"))
                .ToErrorActionResult();
        }
        if (imagesRequest.ImagesToUpload.Count > 0)
        {
            var validateResult = ValidateFiles(imagesRequest.ImagesToUpload, _fileValidatorOptions);
            if (validateResult.IsFailure)
            {
                _logger.LogWarning("Update images validation failed!Errors: {Errors}",
                     validateResult.ToErrorMessages());

                return validateResult.ToErrorActionResult();
            }
        }
        var command = imagesRequest.ToUpdateCommand(volunteerId,petId);

        await using var disposableStreams = new AsyncDisposableCollection(
            command.UploadCommands.Select(c => c.Stream));

        var updatePetImages = await handler.Handle(command, cancelToken);

        return updatePetImages.IsFailure
            ? updatePetImages.ToErrorActionResult()
            : updatePetImages.ToEnvelope();
    }


    //TODO : CHANGE THIS METHOD TO OTHER THAT GETS FROM BODY LIST OF TUPLES: PETID, NEWPOSITION
    //TODO: AND CHANGE HANDLER AND TEST 
    /// <summary>
    /// Change pet position
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="petId"></param>
    /// <param name="newPetPosition"></param>
    /// <param name="handler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    [HttpPost("{volunteerId:Guid}/pet/{petId:Guid}/position/{newPetPosition:int}")]
    public async Task<ActionResult<Envelope>> ChangePetPosition(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromRoute] int newPetPosition,
        [FromServices] ChangePetPositionHandler handler,
        CancellationToken cancellationToken)
    {
        ChangePetPositionCommand command = new(volunteerId, petId, newPetPosition);

        var result = await handler.Handle(command, cancellationToken);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }
}

