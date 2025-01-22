using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Responce;
using PetFamily.Application.Volunteers.CreateVolunteer;
using static PetFamily.API.Extensions.ResultExtensions;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteerController : Controller
{
    private readonly ILogger<VolunteerController> _logger;
    public VolunteerController(ILogger<VolunteerController> logger)
    {
        _logger = logger;   
    }
    [HttpPost]
    public async Task<ActionResult<Envelope>> Create(

        [FromServices]CreateVolunteerHandler handler, 

        [FromBody]CreateVolunteerRequest volunteerRequest,

        CancellationToken cancellationToken=default)

    {
        var handlerResult = await handler.Handler(volunteerRequest, cancellationToken);

        if (handlerResult.IsFailure)
        {
            _logger.LogError("Create volunteer failure!{Errors}", handlerResult.Errors);
            return handlerResult.ToErrorActionResult();
        }

        _logger.LogInformation("Create volunteer success!");

        return CreatedAtAction(nameof(Create), handlerResult.ToEnvelope());
       
    }
}
