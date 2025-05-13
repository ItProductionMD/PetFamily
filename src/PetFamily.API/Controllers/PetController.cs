using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.Queries.Pet.GetPets;
using PetFamily.Application.Queries.Pet.GetPetsFilter;

namespace PetFamily.API.Controllers;

[Route("api/pets")]
[ApiController]
public class PetController : ControllerBase
{
    /// <summary>
    /// Get pets with filters and orders by
    /// </summary>
    /// <param name="handler">'GetPetsQueryHandler'- from services</param>
    /// <param name="pageNumber">from 1 to the last page</param>
    /// <param name="pageSize">from 1 to the 100</param>
    /// <param name="filter">filterBy: VolunteerId,list Of SpeciesId,listOf BreedId,
    ///     VolunteerFullName,PetName,Color,MinAgeInMonth,MaxAgeInMonth,HelpStatus;
    ///     OrderBy: VolunteerName,PetName,Species(order by Species then by Breeds),HelpStatus,Age</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Envelope with GetPetsResponse</returns>
    [HttpGet]
    public async Task<ActionResult<Envelope>> GetPets(
        [FromServices] GetPetsQueryHandler handler,
        [FromServices] GetPetsFilterHandler filterHandler,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] PetsFilter? filter = null,
        [FromQuery] bool hasDataForFilter = false,
        CancellationToken cancellationToken = default)
    {
        var getPetsQuery = new GetPetsQuery(pageNumber, pageSize, filter);      
        var getPetsFilterQuery = new GetPetsFilterQuery(hasDataForFilter);

        var petsTask = handler.Handle(getPetsQuery, cancellationToken);
        var filterTask = filterHandler.Handle(getPetsFilterQuery, cancellationToken);

        await Task.WhenAll(petsTask, filterTask);

        var getPetsResponse = petsTask.Result;
        var getFilterResponse = filterTask.Result;

        if (getPetsResponse.IsFailure)
            return getPetsResponse.ToErrorActionResult();

        if (getFilterResponse.IsFailure)
            return getFilterResponse.ToErrorActionResult();

        getPetsResponse.Data!.SpeciesDtos = getFilterResponse.Data!.SpeciesDtos;

        return getPetsResponse.ToEnvelope();
    }
}
