﻿using FileStorage.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Framework.FormFiles;
using PetFamily.Framework.Utilities;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.Text;
using Volunteers.Presentation.Requests;
using Volunteers.Application;
using Volunteers.Application.Commands.PetManagement.AddPet;
using Volunteers.Application.Commands.PetManagement.AddPetImages;
using Volunteers.Application.Commands.PetManagement.ChangeMainPetImage;
using Volunteers.Application.Commands.PetManagement.ChangePetsOrder;
using Volunteers.Application.Commands.PetManagement.DeletePet;
using Volunteers.Application.Commands.PetManagement.DeletePetImages;
using Volunteers.Application.Commands.PetManagement.RestorePet;
using Volunteers.Application.Commands.PetManagement.UpdatePet;
using Volunteers.Application.Commands.PetManagement.UpdatePetStatus;
using Volunteers.Application.Commands.PetManagement.UpdateSocialNetworks;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.DeleteVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.RestoreVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.SoftDeleteVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;
using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;
using Volunteers.Application.Queries.GetVolunteer;
using Volunteers.Application.Queries.GetVolunteers;
using Volunteers.Application.ResponseDtos;
using static PetFamily.Framework.Extensions.ResultExtensions;

namespace Volunteers.Presentation.Controllers;

[ApiController]
[Route("api/volunteers")]
public class VolunteerController(
    IOptions<PetImagesValidatorOptions> validateFileOptions,
    ILogger<VolunteerController> logger) : Controller
{
    private readonly PetImagesValidatorOptions _fileValidatorOptions = validateFileOptions.Value;
    private readonly ILogger<VolunteerController> _logger = logger;

    //------------------------------------Create volunteer----------------------------------------//
    /// <summary>
    /// Create a volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerRequest"></param>
    /// <param name="cancelToken"></param>
    /// <returns>201</returns>
    [HttpPost]
    public async Task<ActionResult<Envelope>> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest volunteerRequest,
        CancellationToken cancelToken = default)
    {
        var command = volunteerRequest.ToCommand();

        var handlerResult = await handler.Handle(command, cancelToken);

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
    /// <param name="Id"></param>
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
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpGet("{volunteerId}")]
    public async Task<ActionResult<Envelope>> Get(
        [FromServices] GetVolunteerQueryHandler handler,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        GetVolunteerQuery query = new(volunteerId);

        var handlerResult = await handler.Handle(query, cancelToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //------------------------------------SoftDeleteVolunteer-------------------------------------//
    /// <summary>
    /// Soft Delete Volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("{volunteerId}/soft")]
    public async Task<ActionResult<Envelope>> SoftDelete(
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var command = new SoftDeleteVolunteerCommand(volunteerId);

        var handlerResult = await handler.Handle(command, cancelToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //------------------------------------HardDeleteVolunteer-------------------------------------//
    /// <summary>
    ///  Hard Delete Volunteer  
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("{volunteerId}/hard")]
    public async Task<ActionResult<Envelope>> Delete(
       [FromServices] DeleteVolunteerHandler handler,
       [FromRoute] Guid volunteerId,
       CancellationToken cancelToken = default)
    {
        var command = new HardDeleteVolunteerCommand(volunteerId);

        var handlerResult = await handler.Handle(command, cancelToken);

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
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPatch("{volunteerId}/social_networks")]
    public async Task<ActionResult<Envelope>> UpdateSocialNetworks(
        [FromServices] UpdateSocialNetworkHandler handler,
        [FromBody] IEnumerable<SocialNetworksDto> dtos,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var command = new UpdateSocialNetworksCommand(volunteerId, dtos);

        var handlerResult = await handler.Handle(command, cancelToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //-----------------------------------Update Requisites----------------------------------------//
    /// <summary>
    /// Update Requisites
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="dtos"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPatch("{volunteerId}/requisites")]
    public async Task<ActionResult<Envelope>> UpdateRequisites(
        [FromServices] UpdateRequisitesHandler handler,
        [FromBody] IEnumerable<RequisitesDto> dtos,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var command = new UpdateRequisitesCommand(volunteerId, dtos);

        var handlerResult = await handler.Handle(command, cancelToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //------------------------------------RestoreVolunteer----------------------------------------//
    /// <summary>
    /// Restore volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("{volunteerId}/restore")]
    public async Task<ActionResult<Envelope>> Restore(
        [FromServices] RestoreVolunteerHandler handler,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var command = new RestoreVolunteerCommand(volunteerId);

        var handlerResult = await handler.Handle(command, cancelToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //-----------------------------------------AddPet---------------------------------------------//
    /// <summary>
    /// Adds a new pet for a volunteer.
    /// </summary>
    /// <param name="handler">The handler responsible for processing the request.</param>
    /// <param name="id">The ID of the volunteer who owns the pet.</param>
    /// <param name="request">The DTO containing pet details.</param>
    /// <param name="cancelToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// Returns:
    /// - **200 OK** with an `Envelope<AddPetResponse>` if the operation is successful.
    /// - **400 Bad Request** if the request is invalid or contains errors.
    /// - **500 Internal Server Error** if an unexpected error occurs.
    /// </returns>
    [HttpPost("{volunteerId:Guid}/pets")]
    public async Task<ActionResult<Envelope>> Add(
        [FromServices] AddPetHandler handler,
        [FromRoute] Guid volunteerId,
        [FromBody] PetRequest request,
        CancellationToken cancelToken)
    {
        var command = request.ToAddPetCommand(volunteerId);

        var addPetResult = await handler.Handle(command, cancelToken);

        return addPetResult.IsFailure
            ? addPetResult.ToErrorActionResult()
            : Ok(addPetResult.ToEnvelope());
    }

    //-----------------------------------------UpdatePet------------------------------------------//
    [HttpPatch("{volunteerId:Guid}/pets/{petId:Guid}")]
    public async Task<ActionResult<Envelope>> UpdatePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] PetRequest request,
        [FromServices] UpdatePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = request.ToUpdatePetCommand(volunteerId, petId);

        var addPetResult = await handler.Handle(command, cancelToken);

        return addPetResult.IsFailure
            ? addPetResult.ToErrorActionResult()
            : Ok(addPetResult.ToEnvelope());
    }

    //------------------------------------UpdatePetStatus-----------------------------------------//
    [HttpPatch("{volunteerId:Guid}/pets/{petId:Guid}/help_status")]
    public async Task<ActionResult<Envelope>> UpdatePetStatus(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] int helpStatus,
        [FromServices] UpdatePetStatusHandler handler,
        CancellationToken cancelToken)
    {
        var command = new UpdatePetStatusCommand(volunteerId, petId, helpStatus);

        var updateStatusResult = await handler.Handle(command, cancelToken);

        return updateStatusResult.IsFailure
            ? updateStatusResult.ToErrorActionResult()
            : Ok(updateStatusResult.ToEnvelope());

    }
    //------------------------------------DeletePetImages-----------------------------------------//
    /// <summary>
    /// Delete images from Pet
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="petId"></param>
    /// <param name="images"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("{volunteerId:Guid}/pets/{petId:Guid}/images")]
    public async Task<ActionResult<Envelope>> UpdateImages(
        [FromServices] DeletePetImagesHandler handler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] List<string> images,
        CancellationToken cancelToken)
    {
        if (images.Count == 0)
            return UnitResult.Fail(Error.FilesCountIsNull()).ToErrorActionResult();

        var command = new DeletePetImagesCommand(volunteerId, petId, images);

        var deletePetImages = await handler.Handle(command, cancelToken);

        return deletePetImages.IsFailure
            ? deletePetImages.ToErrorActionResult()
            : deletePetImages.ToEnvelope();
    }

    //------------------------------------AddPetImages-----------------------------------------//
    /// <summary>
    /// Delete images from Pet
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="petId"></param>
    /// <param name="images"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpPost("{volunteerId:Guid}/pets/{petId:Guid}/images")]
    public async Task<ActionResult<Envelope>> AddImages(
        [FromServices] AddPetImagesHandler handler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] List<IFormFile> files,
        CancellationToken cancelToken)
    {
        var validate = FormFileValidator.ValidateFiles(files, _fileValidatorOptions);
        if (validate.IsFailure)
            return validate.ToErrorActionResult();

        await using var disposableStreams = new AsyncDisposableCollection();

        var uploadfileCommands = FormFileMapper.ToUploadCommands(files, disposableStreams);

        var command = new AddPetImagesCommand(volunteerId, petId, uploadfileCommands);

        var uploadPetImages = await handler.Handle(command, cancelToken);

        return uploadPetImages.IsFailure
            ? uploadPetImages.ToErrorActionResult()
            : uploadPetImages.ToEnvelope();
    }

    //------------------------------------Change main pet image-----------------------------------//
    [HttpPost("{volunteerId:Guid}/pets{petId:Guid}/images/main_image")]
    public async Task<ActionResult<Envelope>> ChangeMainPetImage(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromQuery] string imageName,
        [FromServices] ChangePetMainImageHandler handler,
        CancellationToken cancelToken)
    {
        var command = new ChangePetMainImageCommand(volunteerId, petId, imageName);

        var result = await handler.Handle(command, cancelToken);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }

    //------------------------------------Change pets order---------------------------------------//
    /// <summary>
    /// Change pets order
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <param name="petId"></param>
    /// <param name="newPetPosition"></param>
    /// <param name="handler"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>

    [HttpPatch("{volunteerId:Guid}/pets")]
    public async Task<ActionResult<Envelope>> ChangePetsOrder(
        [FromRoute] Guid volunteerId,
        [FromBody] ChangePetsOrderRequest request,
        [FromServices] ChangePetsOrderHandler handler,
        CancellationToken cancelToken)
    {
        ChangePetsOrderCommand command = request.ToCommand(volunteerId);

        var result = await handler.Handle(command, cancelToken);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }

    //---------------------------------Soft delete pet--------------------------------------------//
    [HttpDelete("{volunteerId:Guid}/pets/{petId:Guid}/soft")]
    public async Task<ActionResult<Envelope>> SoftDeletePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] SoftDeletePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = new SoftDeletePetCommand(volunteerId, petId);

        var result = await handler.Handle(command, cancelToken);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }

    //---------------------------------Hard delete pet--------------------------------------------//
    [HttpDelete("{volunteerId:Guid}/pets/{petId:Guid}/hard")]
    public async Task<ActionResult<Envelope>> HardDeletePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] HardDeletePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = new HardDeletePetCommand(volunteerId, petId);

        var result = await handler.Handle(command, cancelToken);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }

    //-------------------------------------Restore pet--------------------------------------------//
    [HttpPost("{volunteerId:Guid}/pets/{petId:Guid}/restore")]
    public async Task<ActionResult<Envelope>> RestorePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] RestorePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = new RestorePetCommand(volunteerId, petId);

        var result = await handler.Handle(command, cancelToken);

        return result.IsFailure
            ? result.ToErrorActionResult()
            : result.ToEnvelope();
    }
    //--------------------------------Get Volunteers With Pagination------------------------------//
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="handler"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetVolunteers(
        [FromQuery] int page,
        [FromQuery] string? orderBy,
        [FromQuery] string? orderDirection,
        [FromQuery] int pageSize,
        [FromServices] GetVolunteersQueryHandler handler,
        CancellationToken cancelToken)
    {
        var query = new GetVolunteersQuery(pageSize, page, orderBy, orderDirection);

        var response = await handler.Handle(query, cancelToken);

        return response.IsFailure
            ? response.ToErrorActionResult()
            : response.ToEnvelope();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    [HttpPost("addfakevolunteers")]
    public async Task<ActionResult<Envelope>> AddFakeVolunteers(
        [FromQuery] int count,
        [FromServices] CreateVolunteerHandler handler)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        List<CreateVolunteerCommand> commands = [];
        for (int i = 0; i < count; i++)
        {
            var builder = new StringBuilder();
            var reverseBuilder = new StringBuilder();
            for (int j = 0; j < 10; j++)
            {
                builder.Append(chars[random.Next(chars.Length)]);
            }
            string firstName = builder.ToString();
            string lastName = new string(firstName.ToCharArray().Reverse().ToArray());
            var fakeEmail = i + "@gmail.com";
            var randomNumber = new Random();
            var randomPhoneNumber = randomNumber.Next(10000000, 100000000).ToString();
            string randomPhoneRegion = "+" + randomNumber.Next(20, 400).ToString();
            CreateVolunteerCommand command = new(firstName,
                lastName,
                fakeEmail,
                string.Empty,
                randomPhoneNumber,
                randomPhoneRegion,
                0,
                [],
                []);

            commands.Add(command);
        }
        foreach (var command in commands)
        {
            var result = await handler.Handle(command, CancellationToken.None);
            Console.WriteLine($"Task Result:{result.IsSuccess}");
            if (result.IsFailure)
                Console.WriteLine($"Error:{result.Error.Message},ValidationErrors:" +
                    $"{result.ValidationMessagesToString()}");
        }
        var response = UnitResult.Ok().ToEnvelope();
        return Ok(response);
    }


}

