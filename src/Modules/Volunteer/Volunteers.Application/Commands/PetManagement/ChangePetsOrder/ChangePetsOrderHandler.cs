using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;

namespace Volunteers.Application.Commands.PetManagement.ChangePetsOrder;

public class ChangePetsOrderHandler(
    IVolunteerWriteRepository repository,
    ILogger<ChangePetsOrderHandler> logger) : ICommandHandler<ChangePetsOrderCommand>
{
    public async Task<UnitResult> Handle(ChangePetsOrderCommand cmd, CancellationToken ct)
    {
        if (cmd.petsNewOrder.Count == 0)
            return Result.Fail(Error.InvalidLength("Count of petsNewOrder"));

        var getVolunteer = await repository.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var changePetsOrderResult = volunteer.ChangePetsOrder(cmd.petsNewOrder);

        if (changePetsOrderResult.IsFailure)
        {
            var validationErrors = changePetsOrderResult.ValidationMessagesToString();
            logger.LogError("Change pets order error for volunteer with Id:{id}!Error:{Message}" +
                "ValidationErrors:{ValidationErrors}",
                volunteer.Id, changePetsOrderResult.Error.Message, validationErrors);

            return changePetsOrderResult;
        }
        var result = await repository.SaveAsync(volunteer, ct);
        if (result.IsFailure)
            return result;

        logger.LogInformation("Change pets order for volunteer id:{Id} successful !", volunteer.Id);

        return UnitResult.Ok();
    }
}


