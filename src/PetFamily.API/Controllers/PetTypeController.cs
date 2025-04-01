using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Domain.DomainError;
using PetFamily.API.Dtos;
using PetFamily.Application.Commands.PetTypeManagment.AddPetType;
using PetFamily.Application.Commands.PetTypeManagment.DeleteSpecies;
using PetFamily.Application.Commands.PetTypeManagment.DeleteBreed;
using PetFamily.Application.Queries.PetType.GetListOfSpecies;
using PetFamily.Application.Queries.PetType.GetBreeds;

namespace PetFamily.API.Controllers;

[Route("api/pet_types)")]
[ApiController]
public class PetTypeController : ControllerBase
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
        [FromServices] AddPetTypeHandler handler
        , CancellationToken cancelToken)
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
    [HttpGet("page/{page:int}/order_by/{orderBy}/order_direction/{orderDirection}/page_size/{pageSize:int}")]
    public async Task<ActionResult<Envelope>> GetSpecies(
        [FromRoute] int page,
        [FromRoute] string? orderBy,
        [FromRoute] string? orderDirection,
        [FromRoute] int pageSize,
        [FromServices] GetListOfSpeciesHandler handler,
        CancellationToken cancelToken)
    {
        var query = new GetListOfSpeciesQuery(page, pageSize, orderBy, orderDirection);

        var response = await handler.Handle(query, cancelToken);

        return response.IsFailure
            ? response.ToErrorActionResult()
            : Ok(response.ToEnvelope());
    }

    [HttpGet("species_id/{speciesId}/page/{page:int}/order_by/{orderBy}/order_direction/{orderDirection}/page_size/{pageSize:int}")]
    public async Task<ActionResult<Envelope>> GetBreeds(
        [FromRoute] Guid speciesId,
        [FromRoute] int page,
        [FromRoute] string orderBy,
        [FromRoute] string orderDirection,
        [FromRoute] int pageSize,
        [FromServices] GetBreedsHandler handler,
        CancellationToken cancelToken)
    {
        var query = new GetBreedsQuery(page, pageSize, orderBy, orderDirection, speciesId);

        var response = await handler.Handle(query, cancelToken);

        return response.IsFailure
            ? response.ToErrorActionResult()
            : Ok(response.ToEnvelope());
    }
}
