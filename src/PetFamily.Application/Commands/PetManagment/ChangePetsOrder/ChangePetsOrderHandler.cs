using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetManagment.ChangePetsOrder;

public class ChangePetsOrderHandler(
    IVolunteerWriteRepository volunteerRepository,
    ILogger<ChangePetsOrderHandler> logger) : ICommandHandler<ChangePetsOrderCommand>
{
    private readonly IVolunteerWriteRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<ChangePetsOrderHandler> _logger = logger;

    public async Task<UnitResult> Handle(
        ChangePetsOrderCommand command,
        CancellationToken cancelToken)
    {
        if (command.petsNewOrder.Count == 0)
            return Result.Fail(Error.InvalidLength("Count of petsNewOrder"));

        var getVolunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);
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
        var result = await _volunteerRepository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        _logger.LogInformation("Change pets order for volunteer id:{Id} succesfull!", volunteer.Id);

        return UnitResult.Ok();
    }
}


