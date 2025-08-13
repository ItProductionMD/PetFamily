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
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ILogger<ChangePetsOrderHandler> _logger = logger;

    public async Task<UnitResult> Handle(
        ChangePetsOrderCommand command,
        CancellationToken cancelToken)
    {
        if (command.petsNewOrder.Count == 0)
            return Result.Fail(Error.InvalidLength("Count of petsNewOrder"));

        var getVolunteer = await _repository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var changePetsOrderResult = volunteer.ChangePetsOrder(command.petsNewOrder);

        if (changePetsOrderResult.IsFailure)
        {
            var validationErrors = changePetsOrderResult.ValidationMessagesToString();
            _logger.LogError("Change pets order error for volunteer with Id:{id}!Error:{Message}" +
                "ValidationErrors:{ValidationErrors}",
                volunteer.Id, changePetsOrderResult.Error.Message, validationErrors);

            return changePetsOrderResult;
        }
        var result = await _repository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        _logger.LogInformation("Change pets order for volunteer id:{Id} successful !", volunteer.Id);

        return UnitResult.Ok();
    }
}


