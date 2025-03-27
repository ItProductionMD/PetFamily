using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetFamily.API.Dtos;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;
using static PetFamily.API.Extensions.ResultExtensions;
using static PetFamily.API.Common.AppiValidators.Validators;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.API.Common.Utilities;
using PetFamily.Domain.PetManagment.Root;
using System.Text;
using PetFamily.Domain.PetManagment.ValueObjects;
using Bogus.DataSets;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts.ReadDbContext.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Amazon.S3.Model.Internal.MarshallTransformations;
using PetFamily.Application.Commands.PetManagment.AddPet;
using PetFamily.Application.Commands.PetManagment.ChangePetsOrder;
using PetFamily.Application.Commands.PetManagment.UpdateSocialNetworks;
using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.DeleteVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;
using PetFamily.Application.Commands.VolunteerManagment.RestoreVolunteer;
using PetFamily.Application.Commands.VolunteerManagment.UpdateRequisites;
using PetFamily.Application.Commands.VolunteerManagment.UpdateVolunteer;
using PetFamily.Application.Dtos;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Application.Queries.Volunteer.GetVolunteer;
using PetFamily.Application.Commands.SharedCommands;
using PetFamily.Application.Commands.PetManagment.DeletePetImages;
using PetFamily.Application.Commands.PetManagment.AddPetImages;
using PetFamily.API.Common.AppiValidators;
using PetFamily.Application.Commands.FilesManagment;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("volunteers")]
public class VolunteerController(
    IOptions<FileValidatorOptions> validateFileOptions,
    ILogger<VolunteerController> logger) : Controller
{
    private readonly FileValidatorOptions _fileValidatorOptions = validateFileOptions.Value;
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
        var command = new VolunteerIdCommand(volunteerId);

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
        var command = new VolunteerIdCommand(volunteerId);

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
    [HttpGet("{volunteerId}/restore")]
    public async Task<ActionResult<Envelope>> Restore(
        [FromServices] RestoreVolunteerHandler handler,
        [FromRoute] Guid volunteerId,
        CancellationToken cancelToken = default)
    {
        var command = new VolunteerIdCommand(volunteerId);

        var handlerResult = await handler.Handle(command, cancelToken);

        return handlerResult.IsFailure
            ? handlerResult.ToErrorActionResult()
            : Ok(handlerResult.ToEnvelope());
    }

    //-------------------------------------AddPet---------------------------------------------//
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
        [FromBody] AddPetRequest request,
        CancellationToken cancelToken)
    {
        var command = request.ToCommand(volunteerId);

        var addPetResult = await handler.Handle(command, cancelToken);

        return addPetResult.IsFailure
            ? addPetResult.ToErrorActionResult()
            : Ok(addPetResult.ToEnvelope());
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
        var validate = Validators.ValidateFiles(files, _fileValidatorOptions);
        if (validate.IsFailure)
            return validate.ToErrorActionResult();

        await using var disposableStreams = new AsyncDisposableCollection();

        var uploadfileCommands = Mappers.IFormFilesToUploadCommands(files, disposableStreams);

        var command = new AddPetImagesCommand(volunteerId, petId, uploadfileCommands);

        var uploadPetImages = await handler.Handle(command, cancelToken);

        return uploadPetImages.IsFailure
            ? uploadPetImages.ToErrorActionResult()
            : uploadPetImages.ToEnvelope();
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

    //----------------------------Get Volunteers With Pagination------------------------------//
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="handler"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpGet("page/{page:int}/order_by/{orderBy}/order_direction/{orderDirection}/page_size/{pageSize:int}")]
    public async Task<ActionResult<Envelope>> GetVolunteers(
        [FromRoute] int page,
        [FromRoute] string? orderBy,
        [FromRoute] string? orderDirection,
        [FromRoute] int pageSize,
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
    [HttpPost("addfakevolunteers/{count:int}")]
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

