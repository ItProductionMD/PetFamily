using FileStorage.SharedFramework.IFormFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Framework.HTTPContext.Interfaces;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Utilities;
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
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.DeleteVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.RestoreVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.SoftDeleteVolunteer;
using Volunteers.Application.Commands.VolunteerManagement.UpdateRequisites;
using Volunteers.Application.Commands.VolunteerManagement.UpdateVolunteer;
using Volunteers.Application.Queries.GetVolunteer;
using Volunteers.Application.Queries.GetVolunteers;
using Volunteers.Presentation.Requests;
using static PetFamily.Framework.Extensions.ResultExtensions;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.VolunteerManagement;

namespace Volunteers.Presentation.Controllers;

[ApiController]
[Route("api/volunteers")]
public class VolunteerController(
    IUserContext userContext,
    IOptions<PetImagesValidatorOptions> validateFileOptions) : Controller
{
    private readonly PetImagesValidatorOptions _fileValidatorOptions = validateFileOptions.Value;

    //------------------------------------Create volunteer----------------------------------------//
    [Authorize(Policy = VolunteerCreate)]
    [HttpPost]
    public async Task<ActionResult<Envelope>> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromBody] CreateVolunteerRequest volunteerRequest,
        CancellationToken ct = default)
    {
        var adminId = userContext.GetUserId();
        var command = volunteerRequest.ToCommand(adminId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    //------------------------------------UpdateVolunteer-----------------------------------------//
    /// <summary>
    /// Update a volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="request"></param>
    /// <param name="Id"></param>
    /// <param name="ct"></param>
    /// <returns> Code:200 </returns>
    [Authorize(Policy = VolunteerEdit)]
    [HttpPatch("{volunteerId}")]
    public async Task<ActionResult<Envelope>> Update(
        [FromServices] UpdateVolunteerHandler handler,
        [FromBody] UpdateVolunteerRequest request,
        [FromRoute] Guid volunteerId,
        CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var command = request.ToCommand(userId, volunteerId);

        return (await handler.Handle(command, ct)).ToActionResult();
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
        return (await handler.Handle(query, cancelToken)).ToActionResult();
    }

    //------------------------------------SoftDeleteVolunteer-------------------------------------//
    /// <summary>
    /// Soft Delete Volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerDelete)]
    [HttpDelete("{volunteerId}/soft")]
    public async Task<ActionResult<Envelope>> SoftDelete(
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var command = new SoftDeleteVolunteerCommand(volunteerId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //------------------------------------HardDeleteVolunteer-------------------------------------//
    /// <summary>
    ///  Hard Delete Volunteer  
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerDelete)]
    [HttpDelete("{volunteerId}/hard")]
    public async Task<ActionResult<Envelope>> Delete(
       [FromServices] DeleteVolunteerHandler handler,
       [FromRoute] Guid volunteerId,
       CancellationToken cancelToken = default)
    {
        var command = new HardDeleteVolunteerCommand(volunteerId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //-----------------------------------Update Requisites----------------------------------------//
    /// <summary>
    /// Update Requisites
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="dtos"></param>
    /// <param name="volunteerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerEdit)]
    [HttpPatch("{volunteerId}/requisites")]
    public async Task<ActionResult<Envelope>> UpdateRequisites(
        [FromServices] UpdateRequisitesHandler handler,
        [FromBody] UpdateRequisitesRequest request,
        [FromRoute] Guid volunteerId,
        CancellationToken ct = default)
    {
        var userId = userContext.GetUserId();
        var command = request.ToCommand(userId, volunteerId);

        return (await handler.Handle(command, ct)).ToActionResult();
    }

    //------------------------------------RestoreVolunteer----------------------------------------//
    /// <summary>
    /// Restore volunteer
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="volunteerId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = VolunteerRestore)]
    [HttpPost("{volunteerId}/restore")]
    public async Task<ActionResult<Envelope>> Restore(
        [FromServices] RestoreVolunteerHandler handler,
        [FromRoute] Guid volunteerId,
        CancellationToken ct = default)
    {
        var command = new RestoreVolunteerCommand(volunteerId);
        return (await handler.Handle(command, ct)).ToActionResult();
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
    [Authorize(Policy = VolunteerEdit)]
    [HttpPost("{volunteerId:Guid}/pets")]
    public async Task<ActionResult<Envelope>> Add(
        [FromServices] AddPetHandler handler,
        [FromRoute] Guid volunteerId,
        [FromBody] PetRequest request,
        CancellationToken cancelToken)
    {
        var command = request.ToAddPetCommand(volunteerId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //-----------------------------------------UpdatePet------------------------------------------//
    [Authorize(Policy = VolunteerEdit)]
    [HttpPatch("{volunteerId:Guid}/pets/{petId:Guid}")]
    public async Task<ActionResult<Envelope>> UpdatePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] PetRequest request,
        [FromServices] UpdatePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = request.ToUpdatePetCommand(volunteerId, petId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //------------------------------------UpdatePetStatus-----------------------------------------//
    [Authorize(Policy = VolunteerEdit)]
    [HttpPatch("{volunteerId:Guid}/pets/{petId:Guid}/help_status")]
    public async Task<ActionResult<Envelope>> UpdatePetStatus(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] int helpStatus,
        [FromServices] UpdatePetStatusHandler handler,
        CancellationToken cancelToken)
    {
        var command = new UpdatePetStatusCommand(volunteerId, petId, helpStatus);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
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
    [Authorize(Policy = VolunteerEdit)]
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

        return (await handler.Handle(command, cancelToken)).ToActionResult();
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
    [Authorize(Policy = VolunteerEdit)]
    [HttpPost("{volunteerId:Guid}/pets/{petId:Guid}/images")]
    public async Task<ActionResult<Envelope>> AddImages(
        [FromServices] AddPetImagesHandler handler,
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] List<IFormFile> files,
        CancellationToken cancelToken)
    {
        var validate = IFormFileValidator.ValidateFiles(files, _fileValidatorOptions);
        if (validate.IsFailure)
            return validate.ToErrorActionResult();

        await using var disposableStreams = new AsyncDisposableCollection();

        var uploadfileDtos = files.ToUploadFileDtos(Constants.BUCKET_FOR_PET_IMAGES, disposableStreams);

        var command = new AddPetImagesCommand(volunteerId, petId, uploadfileDtos);

        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //------------------------------------Change main pet image-----------------------------------//
    [Authorize(Policy = VolunteerEdit)]
    [HttpPost("{volunteerId:Guid}/pets{petId:Guid}/images/main_image")]
    public async Task<ActionResult<Envelope>> ChangeMainPetImage(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromQuery] string imageName,
        [FromServices] ChangePetMainImageHandler handler,
        CancellationToken cancelToken)
    {
        var command = new ChangePetMainImageCommand(volunteerId, petId, imageName);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
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
    [Authorize(Policy = VolunteerEdit)]
    [HttpPatch("{volunteerId:Guid}/pets")]
    public async Task<ActionResult<Envelope>> ChangePetsOrder(
        [FromRoute] Guid volunteerId,
        [FromBody] ChangePetsOrderRequest request,
        [FromServices] ChangePetsOrderHandler handler,
        CancellationToken cancelToken)
    {
        ChangePetsOrderCommand command = request.ToCommand(volunteerId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //---------------------------------Soft delete pet--------------------------------------------//
    [Authorize(Policy = VolunteerEdit)]
    [HttpDelete("{volunteerId:Guid}/pets/{petId:Guid}/soft")]
    public async Task<ActionResult<Envelope>> SoftDeletePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] SoftDeletePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = new SoftDeletePetCommand(volunteerId, petId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //---------------------------------Hard delete pet--------------------------------------------//
    [Authorize(Policy = VolunteerEdit)]
    [HttpDelete("{volunteerId:Guid}/pets/{petId:Guid}/hard")]
    public async Task<ActionResult<Envelope>> HardDeletePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] HardDeletePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = new HardDeletePetCommand(volunteerId, petId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
    }

    //-------------------------------------Restore pet--------------------------------------------//
    [Authorize(Policy = VolunteerEdit)]
    [HttpPost("{volunteerId:Guid}/pets/{petId:Guid}/restore")]
    public async Task<ActionResult<Envelope>> RestorePet(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromServices] RestorePetHandler handler,
        CancellationToken cancelToken)
    {
        var command = new RestorePetCommand(volunteerId, petId);
        return (await handler.Handle(command, cancelToken)).ToActionResult();
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
        return (await handler.Handle(query, cancelToken)).ToActionResult();
    }
}

