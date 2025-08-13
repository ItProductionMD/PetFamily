using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.SharedApplication.Commands.PetTypeManagement.AddPetType;
using PetFamily.SharedApplication.Dtos;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetSpecies.Application.Commands.DeleteBreed;
using PetSpecies.Application.Commands.DeleteSpecies;
using PetSpecies.Application.Queries.GetBreedPagedList;
using PetSpecies.Application.Queries.GetSpeciesPagedList;
using PetSpecies.Presentation.Requests;
using static PetFamily.SharedKernel.Authorization.PermissionCodes.SpeciesManagement;

namespace PetSpecies.Presentation.Controllers;

[Route("api/species")]
[ApiController]
public class PetSpeciesController : ControllerBase
{

    /// <summary>
    /// Adds a new pet type  .
    /// </summary>
    /// <param name="request">The data for the new pet species.</param>
    /// <param name="handler">The handler for processing the add species command.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>Returns a response with the operation result (200 OK or an error).</returns>
    [Authorize(Policy = SpeciesCreate)]
    [HttpPost]
    public async Task<ActionResult<Envelope>> AddSpecies(
        [FromBody] AddPetTypeRequest request,
        [FromServices] AddPetTypeHandler handler,
        CancellationToken ct = default)
    {
        var command = request.ToCommand();
        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Deletes a pet species.
    /// </summary>
    /// <param name="speciesId"></param>
    /// <param name="handler"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = SpeciesDelete)]
    [HttpDelete("{speciesId}")]
    public async Task<ActionResult<Envelope>> DeleteSpecies(
        [FromRoute] Guid speciesId,
        [FromServices] DeleteSpeciesHandler handler,
        CancellationToken ct = default)
    {
        var command = new DeleteSpeciesCommand(speciesId);
        return (await handler.Handle(command, ct)).ToActionResult();
    }


    /// <summary>
    /// Deletes a breed.
    /// </summary>
    /// <param name="speciesId"></param>
    /// <param name="breedId"></param>
    /// <param name="handler"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize(Policy = SpeciesEdit)]
    [HttpDelete("{speciesId}/{breedId}")]
    public async Task<ActionResult<Envelope>> DeleteBreed(
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        [FromServices] DeleteBreedHandler handler,
        CancellationToken ct = default)
    {
        var command = new DeleteBreedCommand(speciesId, breedId);
        return (await handler.Handle(command, ct)).ToActionResult();
    }

    /// <summary>
    /// Retrieves a list of pet species.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="orderBy"></param>
    /// <param name="orderDirection"></param>
    /// <param name="pageSize"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetSpecies(
        [FromServices] GetSpeciesPagedListHandler handler,
        [FromQuery] PaginationDto? pagination = default,
        [FromQuery] SpeciesFilterDto? filterDto = default,
        CancellationToken ct = default)
    {

        var query = new GetSpeciesPagedListQuery(filterDto, pagination);
        return (await handler.Handle(query, ct)).ToActionResult();
    }

    [HttpGet("breeds")]
    public async Task<ActionResult<Envelope>> GetBreeds(
        [FromServices] GetBreedsHandler handler,
        [FromQuery] PaginationDto? pagination,
        [FromQuery] BreedFilterDto? filter,
        CancellationToken ct = default)
    {
        var query = new GetBreedPageListQuery(pagination, filter);
        return (await handler.Handle(query, ct)).ToActionResult();
    }
}
