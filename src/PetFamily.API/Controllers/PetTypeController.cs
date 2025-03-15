using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Shared.Validations;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationMessages;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using PetFamily.API.Dtos;
using PetFamily.Application.Commands.PetTypeManagment;

namespace PetFamily.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PetTypeController : ControllerBase
{

    /// <summary>
    /// Adds a new pet type  .
    /// </summary>
    /// <param name="request">The data for the new pet species.</param>
    /// <param name="handler">The handler for processing the add species command.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Returns a response with the operation result (200 OK or an error).</returns>
    [HttpPost]
    public async Task<ActionResult<Envelope>> AddSpecies(
        [FromBody] AddPetTypeRequest request,
        [FromServices]AddSpeciesHandler handler
        ,CancellationToken cancellationToken)
    {
        List<Error> validationErrors = [];

        var addPetType = await handler.Handle(request.ToCommand(),cancellationToken);

         return addPetType.IsFailure
            ? addPetType.ToErrorActionResult()
            : Ok(addPetType.ToEnvelope());
    }
}
