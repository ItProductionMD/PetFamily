using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Dtos;
using PetFamily.Application.SharedValidations;
using System.Data;
using Microsoft.Extensions.Options;
using PetFamily.Application.Pets.CreatePet;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Domain.DomainError;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Commands;
using System.Security.Cryptography.X509Certificates;
using PetFamily.Application.FilesManagment.Dtos;
using System.Collections.Generic;
using PetFamily.Domain.VolunteerAggregates.ValueObjects;
using System.Data.Common;
using PetFamily.Application.Pets.UpdatePetImages;
using PetFamily.Domain.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Routing.Constraints;
using static PetFamily.API.AppiValidators.Validators;
using Error = PetFamily.Domain.DomainError.Error;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.API.Mappers;

namespace PetFamily.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PetController(
    IOptions<FileValidatorOptions> validateFileOptions,
    ILogger<PetController> logger) : ControllerBase
{
    private readonly FileValidatorOptions _fileValidatorOptions = validateFileOptions.Value;
    private readonly ILogger<PetController> _logger = logger;
    private const string FOLDER_NAME = "test";

    //-----------------------------------------Add pet--------------------------------------------//
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
    [HttpPost("{volunteerId:Guid}")]
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

    //-------------------------------------Update Pet Images--------------------------------------//
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
    [HttpPost("{petId:Guid}/Images")]
    public async Task<ActionResult<Envelope>> UpdateImages(
        [FromServices] UpdatePetImagesHandler handler,
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
                _logger.LogWarning("UpdateImages validation failed: {ValidationErrors}",
                     validateResult.ConcateErrorMessages()); 

                return validateResult.ToErrorActionResult();
            }

            uploadCommands = imagesRequest.ImagesToUpload.MapToUploadFileCommandList();
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
