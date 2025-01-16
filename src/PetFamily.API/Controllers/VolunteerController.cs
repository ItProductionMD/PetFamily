using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.Application.Volunteers.CreateVolunteer;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteerController : Controller
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromServices]CreateVolunteerHandler handler, 
        [FromBody]CreateVolunteerRequest volunteerRequest,
        CancellationToken cancellationToken=default)
    {

        var handlerResult = await handler.Handler(volunteerRequest,cancellationToken);
        if (handlerResult.IsFailure)
            return handlerResult.Error!.ToActionResult();

        return Ok(handlerResult.Data);
       
    }
}
