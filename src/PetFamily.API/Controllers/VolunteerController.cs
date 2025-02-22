using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Mappers;
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
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.Results;
using static PetFamily.API.Extensions.ResultExtensions;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;
using static PetFamily.API.AppiValidators.Validators;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.UpdatePetImages;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteerController(
    IOptions<FileValidatorOptions> validateFileOptions,
    ILogger<VolunteerController> logger) : Controller
{
    private readonly FileValidatorOptions _fileValidatorOptions = validateFileOptions.Value;
    private readonly ILogger<VolunteerController> _logger = logger;
    private const string FOLDER_NAME = "test";
   
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

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
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
            _logger.LogError("Update volunteer with id:{id} failure!{Errors}",
                id,handlerResult.ConcateErrorMessages());

            return handlerResult.ToErrorActionResult();
        }
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
        [FromBody] IEnumerable<RequisitesRequest> dtos,
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
    /// <param name="petDto">The DTO containing pet details.</param>
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
        [FromBody] PetDto petDto,
        CancellationToken cancellationToken)
    {

        var addCommand = petDto.MapToAddPetCommand(volunteerId);

        var addPetResult = await handler.Handle(volunteerId, addCommand, cancellationToken);

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
    /// <param name="cancellationToken">Token to cancel the request</param>
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
        CancellationToken cancellationToken)
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

        List<UploadFileCommand> uploadCommands = [];
        List<DeleteFileCommand> deleteCommands = [];

        if (imagesRequest.ImagesToUpload.Count > 0)
        {
            //Added Validation Of IFormFiles
            var validateResult = ValidateFiles(imagesRequest.ImagesToUpload, _fileValidatorOptions);

            if (validateResult.IsFailure)
            {
                _logger.LogWarning("Update images validation failed!Errors: {Errors}",
                     validateResult.ConcateErrorMessages());

                return validateResult.ToErrorActionResult();
            }

            uploadCommands = imagesRequest.ImagesToUpload.MapToUploadFileCommands();
        }
        if (imagesRequest.ImagesToDelete.Count > 0)
        {
            deleteCommands = imagesRequest.ImagesToDelete
                .Select(i => new DeleteFileCommand(i)).ToList();
        }
        try
        {
            var updatePetImages = await handler.Handle(
                FOLDER_NAME,
                volunteerId,
                petId,
                uploadCommands,
                deleteCommands,
                cancellationToken);

            return updatePetImages.IsFailure
                ? updatePetImages.ToErrorActionResult()
                : updatePetImages.ToEnvelope();
        }
        finally
        {
            foreach (var item in uploadCommands)
                item.Stream.Dispose();
        }

    }
}

