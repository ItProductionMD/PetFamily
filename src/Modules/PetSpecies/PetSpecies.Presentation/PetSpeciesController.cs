using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetFamily.Application.Commands.PetTypeManagement.AddPetType;
using PetFamily.Application.Dtos;
using PetSpecies.Presentation.Requests;
using PetSpecies.Application.Commands.DeleteBreed;
using PetSpecies.Application.Commands.DeleteSpecies;
using PetSpecies.Application.Queries.GetBreedPagedList;
using PetSpecies.Application.Queries.GetSpeciesPagedList;

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
    /// <param name="cancelToken">A token to cancel the operation.</param>
    /// <returns>Returns a response with the operation result (200 OK or an error).</returns>
    [HttpPost]
    public async Task<ActionResult<Envelope>> AddSpecies(
        [FromBody] AddPetTypeRequest request,
        [FromServices] AddPetTypeHandler handler,
        CancellationToken cancelToken)
    {
        var addPetType = await handler.Handle(request.ToCommand(), cancelToken);

        return addPetType.IsFailure
           ? addPetType.ToErrorActionResult()
           : Ok(addPetType.ToEnvelope());
    }

    /// <summary>
    /// Deletes a pet species.
    /// </summary>
    /// <param name="speciesId"></param>
    /// <param name="handler"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("{speciesId}")]
    public async Task<ActionResult<Envelope>> DeleteSpecies(
        [FromRoute] Guid speciesId,
        [FromServices] DeleteSpeciesHandler handler,
        CancellationToken cancelToken)
    {

        var deleteSpecies = await handler.Handle(new(speciesId), cancelToken);

        return deleteSpecies.IsFailure
            ? deleteSpecies.ToErrorActionResult()
            : Ok(deleteSpecies.ToEnvelope());
    }


    /// <summary>
    /// Deletes a breed.
    /// </summary>
    /// <param name="speciesId"></param>
    /// <param name="breedId"></param>
    /// <param name="handler"></param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    [HttpDelete("{speciesId}/{breedId}")]
    public async Task<ActionResult<Envelope>> DeleteBreed(
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        [FromServices] DeleteBreedHandler handler,
        CancellationToken cancelToken)
    {
        var deleteBreed = await handler.Handle(new(speciesId, breedId), cancelToken);

        return deleteBreed.IsFailure
            ? deleteBreed.ToErrorActionResult()
            : Ok(deleteBreed.ToEnvelope());
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
        CancellationToken cancelToken = default)
    {

        var query = new GetSpeciesPagedListQuery(filterDto, pagination);

        var response = await handler.Handle(query, cancelToken);

        return response.IsFailure
            ? response.ToErrorActionResult()
            : Ok(response.ToEnvelope());
    }

    [HttpGet("{speciesId}")]
    public async Task<ActionResult<Envelope>> GetBreeds(
        [FromServices] GetBreedsHandler handler,
        [FromQuery] PaginationDto? pagination,
        [FromQuery] BreedFilterDto? filter,
        CancellationToken cancelToken = default)
    {
        var query = new GetBreedPageListQuery(pagination, filter);

        var response = await handler.Handle(query, cancelToken);

        return response.IsFailure
            ? response.ToErrorActionResult()
            : Ok(response.ToEnvelope());
    }
}
