using Microsoft.AspNetCore.Mvc;
using PetFamily.Framework;
using PetFamily.Framework.Extensions;
using PetSpecies.Public.IContracts;
using Volunteers.Application.Queries.GetPets;
using Volunteers.Application.Queries.GetPets.ForFilter;

namespace Volunteers.Presentation.Controllers;

[Route("api/pets")]
[ApiController]
public class PetController(ISpeciesQueryContract speciesForFilter) : Controller
{
    private readonly ISpeciesQueryContract _speciesForFilter = speciesForFilter;

    /// <summary>
    /// Get pets with filters and orders by
    /// </summary>
    /// <param name="petHandler">'GetPetsQueryHandler'- from services</param>
    /// <param name="pageNumber">from 1 to the last page</param>
    /// <param name="pageSize">from 1 to the 100</param>
    /// <param name="filter">filterBy: VolunteerId,list Of SpeciesId,listOf BreedId,
    ///     VolunteerFullName,PetName,Color,MinAgeInMonth,MaxAgeInMonth,HelpStatus;
    ///     OrderBy: VolunteerName,PetName,Species(order by Species then by Breeds),HelpStatus,Age</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Envelope with GetPetsResponse</returns>
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetPets(
        [FromServices] GetPetsQueryHandler petHandler,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] PetsFilter? filter = null,
        [FromQuery] bool hasDataForFilter = false,
        CancellationToken cancellationToken = default)
    {
        var petsQuery = new GetPetsQuery(pageNumber, pageSize, filter);
        var petsTask = petHandler.Handle(petsQuery, cancellationToken);

        var speciesTask = _speciesForFilter.GetAllSpeciesAsync();

        await Task.WhenAll(petsTask, speciesTask);

        var getPetsResult = petsTask.Result;
        if (getPetsResult.IsFailure)
            return getPetsResult.ToErrorActionResult();

        var petsResponse = getPetsResult.Data!;

        petsResponse.SpeciesDtos = speciesTask.Result ?? [];

        return getPetsResult.ToEnvelope();
    }
}
